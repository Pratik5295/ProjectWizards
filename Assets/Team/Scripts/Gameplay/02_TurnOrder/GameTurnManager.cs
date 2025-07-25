using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Gameplay.ObjectiveSystem;
using Team.Gameplay.TurnSystem;
using Team.UI.Gameplay;
using UnityEngine;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float PauseBetweenTurn = 0.4f;
    }
}

namespace Team.Managers
{
    [DefaultExecutionOrder(4)]
    public class GameTurnManager : MonoBehaviour
    {
        public static GameTurnManager Instance = null;

        #region Variables
        [Header("Components")]
        private Queue<GameTurn> turnQueue;
        private Stack<GameTurn> _historyStack = new Stack<GameTurn>();

        public List<GameObject> DestroyedObjects = new List<GameObject>();
        public List<GameObject> Obstacles = new List<GameObject>();
        public List<GridTile> ChangedTiles = new List<GridTile>();
        public List<GridTile> RotatedTiles = new List<GridTile>();

        public List<GameObject> originalOrder = new List<GameObject>();
        public List<GameObject> currentTurnOrder = new List<GameObject>();

        [SerializeField]
        private TurnHolder turnHolder;

        [Space(5)]
        [Header("Breakpoint System Variables")]
        [SerializeField]
        private GameBreakpoint breaker;
        [SerializeField]
        private int breakerIndex = 0;
        [SerializeField]
        private bool breakpoint = false;
        [SerializeField]
        private bool playedTillBreaker = false;

        public bool HasCharacterTurns => turnQueue?.Count > 0;
        private bool isQueueLoaded = false;

        // Cancellation token for managing async operations
        private CancellationTokenSource _operationCancellationTokenSource;
        private readonly object _lockObject = new object();

        // State tracking for preventing race conditions
        private bool _isProcessingTurns = false;
        private bool _isResetting = false;
        //Added by Rían sorry Pratikkk
        public bool _areCharNumbersActive { get; private set; } = false;


        public Action OnTurnsProcessingEvent;
        public Action OnAllTurnsCompleted;
        public Action OnResetLastTurnCompleted;
        public Action OnPlayedTillBreakpoint;
        public Action OnRestartProcessingEvent;

        //Also added by Rían sorrrrrrrrry Pratikkkk
        public Action<bool> OnCharacterNumberVisibilityChanged;

        [Space(5)]
        [Header("Turn Identifier Images")]

        [SerializeField]
        private List<Sprite> turnIdentifierAtlas = new List<Sprite>();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            CancelCurrentOperations();
        }

        private void InitializeManager()
        {
            turnQueue = new Queue<GameTurn>();
            _historyStack = new Stack<GameTurn>();
            _operationCancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Public Methods

        public void HasBreakpoint(bool _hasBreakPoint)
        {
            breakpoint = _hasBreakPoint;
            if (breaker != null)
            {
                breaker.gameObject.SetActive(breakpoint);
            }
        }

        public async Task LoadQueueAsync(CancellationToken cancellationToken = default)
        {
            if (_isResetting) return;

            //LoadObstacleData();
            //breakerIndex = breaker != null ? breaker.transform.GetSiblingIndex() : 0;

            lock (_lockObject)
            {
                turnQueue?.Clear();
                if (turnQueue == null)
                {
                    turnQueue = new Queue<GameTurn>();
                }
            }

            // Wait for end of frame
            await Task.Yield();

            if (cancellationToken.IsCancellationRequested) return;

            foreach (var unit in currentTurnOrder)
            {
                if (cancellationToken.IsCancellationRequested) return;

                if (unit != null && unit.TryGetComponent<GameTurn>(out var gameTurn))
                {
                    lock (_lockObject)
                    {
                        turnQueue.Enqueue(gameTurn);
                    }
                }
            }

            isQueueLoaded = true;
        }

        /// <summary>
        /// BreakPoint Method. Currently Unused
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task LoadQueueFromIndexAsync(int index, CancellationToken cancellationToken = default)
        {
            if (_isResetting) return;

            //LoadObstacleData();

            lock (_lockObject)
            {
                turnQueue?.Clear();
            }

            await Task.Yield();

            if (cancellationToken.IsCancellationRequested) return;

            for (int i = index; i < currentTurnOrder.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested) return;

                if (currentTurnOrder[i] != null && currentTurnOrder[i].TryGetComponent<GameTurn>(out var gameTurn))
                {
                    lock (_lockObject)
                    {
                        turnQueue.Enqueue(gameTurn);
                    }
                }
            }

            isQueueLoaded = true;
        }

        public void EmptyQueue()
        {
            lock (_lockObject)
            {
                _historyStack?.Clear();
                turnQueue?.Clear();
                originalOrder?.Clear();
                currentTurnOrder?.Clear();
                DestroyedObjects?.Clear();
                Obstacles?.Clear();
                RotatedTiles?.Clear();
                ChangedTiles?.Clear();
                isQueueLoaded = false;
            }
        }

        public void ForceRebuildTurns()
        {
            if (turnHolder?.transform.childCount == 0)
            {
                Debug.LogError("Character turns are missing");
                return;
            }

            lock (_lockObject)
            {
                currentTurnOrder.Clear();
                for (int i = 0; i < turnHolder.transform.childCount; i++)
                {
                    var child = turnHolder.transform.GetChild(i);
                    if (child != null && child.gameObject.TryGetComponent<GameTurn>(out var turn))
                    {
                        currentTurnOrder.Add(child.gameObject);
                    }
                }
            }
        }

        public void AddCharacterToTurnOrder(GameObject _turnObject)
        {
            if (_turnObject == null) return;

            lock (_lockObject)
            {
                if (!originalOrder.Contains(_turnObject))
                {
                    originalOrder.Add(_turnObject);
                }

                if (!currentTurnOrder.Contains(_turnObject))
                {
                    currentTurnOrder.Add(_turnObject);
                }
            }
        }

        public void AddDestroyedObject(GameObject _destroyedObject)
        {
            if (_destroyedObject == null) return;

            lock (_lockObject)
            {
                DestroyedObjects.Add(_destroyedObject);
            }
        }

        public void AddChangedTile(GridTile _changedTile)
        {
            if (_changedTile == null) return;

            lock (_lockObject)
            {
                ChangedTiles.Add(_changedTile);
            }
        }

        public void AddRotatedTile(GridTile _changedTile)
        {
            if (_changedTile == null) return;
            if (RotatedTiles.Contains(_changedTile)) return;

            RotatedTiles.Add(_changedTile);
        }

        public int GetIndexFor(GameObject _turn)
        {
            return currentTurnOrder.IndexOf(_turn);
        }

        public Sprite GetTurnIdentifierSprite(GameObject _turn)
        {
            int index = GetIndexFor(_turn);

            return turnIdentifierAtlas[index];
        }

        public void ToggleCharacterNumberVisibility(bool isActive)
        {

            OnCharacterNumberVisibilityChanged.Invoke(isActive);

            _areCharNumbersActive = isActive;

        }



        #endregion

        #region Private Methods

        private void LoadObstacleData()
        {
            Obstacles.Clear();

            var gridManager = GridManager.Instance;
            if (gridManager?.Obstacles?.Count > 0)
            {
                foreach (var obs in gridManager.Obstacles)
                {
                    if (obs != null)
                    {
                        Obstacles.Add(obs);
                    }
                }
            }
        }

        private void ResetObstacles()
        {
            foreach (var obs in Obstacles)
            {
                if (obs != null && obs.TryGetComponent<Base_Obstacle>(out var obsData))
                {
                    obsData.ResetToStart();
                }
            }

            Obstacles.Clear();
        }

        private void ResetTileData()
        {
            FireSpread.Instance?.ResetOilTiles();
        }

        private void ResetBreakpointSystem()
        {
            playedTillBreaker = false;
            breakerIndex = 0;
        }

        private void CancelCurrentOperations()
        {
            _operationCancellationTokenSource?.Cancel();
            _operationCancellationTokenSource?.Dispose();
            _operationCancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Turn Execution Methods

        [ContextMenu("Play All Turns")]
        public async void PlayTurns()
        {
            if (_isProcessingTurns || _isResetting) return;

            _isProcessingTurns = true;
            OnTurnsProcessingEvent?.Invoke();

            try
            {
                await PlayTurnsAsync(_operationCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Turn processing was cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during turn processing: {ex.Message}");
            }
            finally
            {
                _isProcessingTurns = false;
            }
        }

        private async Task PlayTurnsAsync(CancellationToken cancellationToken)
        {
            if (!breakpoint)
            {
                await PlayAllTurnsAsync(cancellationToken);
            }
            else
            {
                await PlayBreakpointTurnsAsync(cancellationToken);
            }
        }

        private async Task PlayBreakpointTurnsAsync(CancellationToken cancellationToken)
        {
            if (!playedTillBreaker)
            {
                await LoadQueueAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;

                bool playAllTurns = IsBreakerAtExtremes();
                if (playAllTurns)
                {
                    await PlayAllTurnsAsync(cancellationToken);
                }
                else
                {
                    await PlayFirstSectionAsync(cancellationToken);
                }
            }
            else
            {
                await PlayLastSectionAsync(cancellationToken);
            }
        }

        private async Task PlayFirstSectionAsync(CancellationToken cancellationToken)
        {
            int currentIndex = 0;

            while (currentIndex < breakerIndex && !cancellationToken.IsCancellationRequested)
            {
                Debug.Log($"Runner index: {currentIndex}");
                await RunNextTurnAsync(cancellationToken);
                currentIndex++;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                playedTillBreaker = true;
                OnPlayedTillBreakpoint?.Invoke();
                turnHolder?.BreakpointInitiate(currentIndex);
                breaker?.MakeUnInteractable();
            }
        }

        private async Task PlayLastSectionAsync(CancellationToken cancellationToken)
        {
            int currentIndex = breakerIndex;

            await LoadQueueFromIndexAsync(currentIndex, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;

            while (HasCharacterTurns && !cancellationToken.IsCancellationRequested)
            {
                Debug.Log($"Runner index: {currentIndex}");
                await RunNextTurnAsync(cancellationToken);
                currentIndex++;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Completed the entire breakpoint system loop");
                OnAllTurnsCompleted?.Invoke();
                turnHolder?.Reset();
                breaker?.MakeInteractable();
            }
        }

        private bool IsBreakerAtExtremes()
        {
            bool playAllTurns = breakerIndex == 0 || breakerIndex >= (turnQueue?.Count ?? 0);
            return playAllTurns;
        }

        private async Task PlayAllTurnsAsync(CancellationToken cancellationToken)
        {
            await LoadQueueAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;

            while (HasCharacterTurns && !cancellationToken.IsCancellationRequested)
            {
                await RunNextTurnAsync(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                Debug.Log("All turns completed.");
                OnAllTurnsCompleted?.Invoke();
            }
        }

        private async Task RunNextTurnAsync(CancellationToken cancellationToken)
        {
            GameTurn turn = null;

            lock (_lockObject)
            {
                if (turnQueue?.Count > 0)
                {
                    turn = turnQueue.Dequeue();
                }
            }

            if (turn == null || cancellationToken.IsCancellationRequested) return;

            UIGameCard gameCard = turn.GetComponent<UIGameCard>();

            if (turn.IsAlive())
            {
                await turn.PerformAsync();
                if (cancellationToken.IsCancellationRequested) return;

                await Task.Delay(TimeSpan.FromSeconds(MetaConstants.PauseBetweenTurn), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;

                lock (_lockObject)
                {
                    _historyStack.Push(turn);
                }

                gameCard?.MakeUninteractable();
            }
            else
            {
                gameCard?.MakeUninteractable();
            }
        }

        [ContextMenu("Play Next Turn")]
        public async void PlayNextTurn()
        {
            if (_isProcessingTurns || _isResetting) return;

            _isProcessingTurns = true;
            OnTurnsProcessingEvent?.Invoke();

            try
            {
                if (!isQueueLoaded)
                {
                    await LoadQueueAsync(_operationCancellationTokenSource.Token);
                }

                if (HasCharacterTurns)
                {
                    await RunNextTurnAsync(_operationCancellationTokenSource.Token);
                }
                else
                {
                    isQueueLoaded = false;
                    OnAllTurnsCompleted?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Next turn processing was cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during next turn processing: {ex.Message}");
            }
            finally
            {
                _isProcessingTurns = false;
            }
        }

        #endregion

        #region Reset Methods

        public void CleanUpLevel()
        {
            foreach (var turn in currentTurnOrder)
            {
                DestroyImmediate(turn.gameObject);
            }
        }

        [ContextMenu("Reset Turns")]
        public async void ResetAllTurns()
        {
            if (_isResetting) return;

            OnRestartProcessingEvent?.Invoke();

            _isResetting = true;
            CancelCurrentOperations();
            OnTurnsProcessingEvent?.Invoke();

            try
            {
                await ResetAllTurnsAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during reset: {ex.Message}");
            }
            finally
            {
                _isResetting = false;
            }
        }

        private async Task ResetAllTurnsAsync()
        {
            // Reset all moves performed by the characters
            while (_historyStack.Count > 0)
            {
                GameTurn turn = _historyStack.Pop();
                if (turn != null)
                {
                    if (turn.TryGetComponent<UIGameCard>(out var gameCard))
                    {
                        gameCard?.MakeInteractable();
                    }

                  
                    await turn.Undo();
                }
            }

            //FUTURE SCARE: Turn Order is getting reset to OG
            // Reset the turn order to original ui order
            //lock (_lockObject)
            //{
            //    //currentTurnOrder.Clear();
            //    //for (int i = 0; i < originalOrder.Count; i++)
            //    //{
            //    //    var turn = originalOrder[i];
            //    //    if (turn != null)
            //    //    {
            //    //        currentTurnOrder.Add(turn);
            //    //        turn.transform.SetSiblingIndex(i);
            //    //    }
            //    //}
            //}

           // ResetBreaker();

            // Use async delay instead of Invoke
            await Task.Delay(TimeSpan.FromSeconds(2f));

            await CompleteResetAsync();
        }

        private async Task CompleteResetAsync()
        {
            // All Unity operations must stay on main thread
            ResetDestroyedEntities();

            // Reset objectives
            LevelObjectiveManager.Instance?.ResetAllObjectives();

            //ResetObstacles();

            // Reset characters
            ResetCharactersToStart();

            // Reset tile data
            ResetTileData();

            // Reset changed tiles
            ResetChangedTiles();

            //Reset Any tiles that have been rotated back to their initial starting points.
            ResetRotatedTiles();

            //Reset breakpoint system
            //ResetBreakpointSystem();

            //Turn all cards interactable
            TurnAllCardsInteractable();

            OnResetLastTurnCompleted?.Invoke();
            isQueueLoaded = false;

            Debug.Log("Completed reset");

            // Add a small delay to ensure all operations complete
            await Task.Yield();
        }


        private void ResetDestroyedEntities()
        {
            for (int i = DestroyedObjects.Count - 1; i >= 0; i--)
            {
                var destroyedObj = DestroyedObjects[i];
                if (destroyedObj == null) continue;

                if (destroyedObj.CompareTag(MetaConstants.CharacterTag))
                {
                    var character = destroyedObj.GetComponent<Base_Ch>();
                    if (character != null)
                    {
                        character.EnableObject();
                        character.resetCharState(true);
                        //character.UndoAction();
                    }
                }
                else
                {
                    var obstacle = destroyedObj.GetComponent<Base_Obstacle>();
                    if (obstacle != null)
                    {
                        obstacle.EnableObject();

                        var moveable = destroyedObj.GetComponent<MoveableObstacle>();
                        if (moveable != null)
                        {
                            moveable.UndoAction();
                        }
                        else
                        {
                            var obstacleData = destroyedObj.GetComponent<ObstacleData>();
                            if (obstacleData != null)
                            {
                                obstacleData.EnableObject();
                                var character = destroyedObj.GetComponent<Base_Ch>();
                                if (character != null)
                                {
                                    character.resetCharState(true);
                                    //character.UndoAction();
                                }
                            }
                        }
                    }
                }
            }
            DestroyedObjects.Clear();
        }

        private void ResetChangedTiles()
        {
            foreach (var tile in ChangedTiles)
            {
                tile?.ResetTypeToDefault();
            }
            ChangedTiles.Clear();
        }

        private void ResetRotatedTiles()
        {
            foreach (var tile in RotatedTiles)
            {
                tile?.ResetTileToOrigin();
            }
            RotatedTiles.Clear();
        }

        private void ResetCharactersToStart()
        {
            CharacterManager.Instance?.ResetAllCharacters();
        }

        private void ResetBreaker()
        {
            if (breaker != null)
            {
                breaker.transform.SetAsFirstSibling();
                breaker.MakeInteractable();
            }
            breakerIndex = 0;
        }

        #endregion

        #region Turn Holder Section

        public void OnCharactersLoaded()
        {
            turnHolder?.InitializeComplete();
            OnResetLastTurnCompleted?.Invoke();

            PopulateOrderText();
        }

        private void PopulateOrderText()
        {
            int orderIndex = 1;
            foreach(var turn in originalOrder)
            {
                if(turn.TryGetComponent<UIGameCard>(out var card))
                {
                    card.UpdateCharacterNumber(orderIndex);
                    orderIndex++;
                }
            }
        }

        private void TurnAllCardsInteractable()
        {
            //Do it after all characters are reset

            foreach(var turn in originalOrder)
            {
                if(turn.TryGetComponent<UIGameCard>(out var card))
                {
                    card.MakeInteractable();
                }
            }
        }

        #endregion

    }
}