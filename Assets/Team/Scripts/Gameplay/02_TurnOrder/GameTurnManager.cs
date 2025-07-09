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
        public const float PauseBetweenTurn = 2f;
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
        public Dictionary<TileID,TileType> ChangedTiles = new Dictionary<TileID, TileType>();

        public List<GameObject> originalOrder = new List<GameObject>();
        public List<GameObject> currentTurnOrder = new List<GameObject>();

        [SerializeField] private TurnHolder turnHolder;
        [SerializeField] private GameBreakpoint breaker;
        [SerializeField] private int breakerIndex = 0;
        [SerializeField] private bool breakpoint = false;
        [SerializeField] private bool playedTillBreaker = false;

        // Reset state management
        private bool isResetting = false;
        private CancellationTokenSource _cancellationToken;
        private CancellationTokenSource _resetCancellationToken;
        private bool isQueueLoaded = false;

        public bool HasCharacterTurns => turnQueue.Count > 0;
        public bool IsResetting => isResetting;
        public bool isPlaying = false; //Bool flag to show that its playing

        // Events
        public Action OnTurnsProcessingEvent;
        public Action OnAllTurnsCompleted;
        public Action OnResetLastTurnCompleted;
        public Action OnPlayedTillBreakpoint;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            // Cleanup cancellation tokens
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _resetCancellationToken?.Cancel();
            _resetCancellationToken?.Dispose();
        }
        #endregion

        #region Public Methods
        public void HasBreakpoint(bool _hasBreakPoint)
        {
            breakpoint = _hasBreakPoint;
            breaker.gameObject.SetActive(breakpoint);
        }

        public async Task LoadQueue()
        {
            if (isResetting) return;

            LoadObstacleData();
            breakerIndex = breaker.transform.GetSiblingIndex();

            if (turnQueue == null)
            {
                turnQueue = new Queue<GameTurn>();
            }
            else
            {
                turnQueue.Clear();
            }

            await Task.Yield();

            foreach (var unit in currentTurnOrder)
            {
                if (unit.TryGetComponent<GameTurn>(out var gameTurn))
                {
                    turnQueue.Enqueue(gameTurn);
                }
            }

            isQueueLoaded = true;
        }

        public async Task LoadQueueFromIndex(int index)
        {
            if (isResetting) return;

            LoadObstacleData();
            turnQueue.Clear();

            await Task.Yield();

            for (int i = index; i < currentTurnOrder.Count; i++)
            {
                if (currentTurnOrder[i].TryGetComponent<GameTurn>(out var gameTurn))
                {
                    turnQueue.Enqueue(gameTurn);
                }
            }

            isQueueLoaded = true;
        }

        public void EmptyQueue()
        {
            _historyStack.Clear();

            if (turnQueue != null)
            {
                turnQueue.Clear();
            }

            originalOrder.Clear();
            currentTurnOrder.Clear();
            isQueueLoaded = false;
        }

        public void ForceRebuildTurns()
        {
            if (turnHolder.transform.childCount == 0)
            {
                Debug.LogError("Character turns are missing");
                return;
            }

            currentTurnOrder.Clear();
            for (int i = 0; i < turnHolder.transform.childCount; i++)
            {
                currentTurnOrder.Add(turnHolder.transform.GetChild(i).gameObject);
            }
        }

        public void AddCharacterToTurnOrder(GameObject _turnObject)
        {
            if (originalOrder.Contains(_turnObject)) return;
            originalOrder.Add(_turnObject);

            if (currentTurnOrder.Contains(_turnObject)) return;
            currentTurnOrder.Add(_turnObject);
        }

        public void AddDestroyedObject(GameObject _destroyedObject)
        {
            if (!DestroyedObjects.Contains(_destroyedObject))
            {
                DestroyedObjects.Add(_destroyedObject);
            }
        }

        public void AddChangedTile(GridTile _changedTile,TileType _originalType)
        {
            if (!ChangedTiles.ContainsKey(_changedTile.TileID))
            {
                ChangedTiles.Add(_changedTile.TileID, _originalType);
            }
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
                    Obstacles.Add(obs);
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
        }

        private void ResetTileData()
        {
            FireSpread.Instance?.ResetOilTiles();
        }

        private void ResetBreakpointSystem()
        {
            breakpoint = false;
            playedTillBreaker = false;
            breakerIndex = 0;
        }
        #endregion

        #region Turn Execution Methods
        [ContextMenu("Play All Turns")]
        public async void PlayTurns()
        {
            if (isResetting) return;

            isPlaying = true;

            OnTurnsProcessingEvent?.Invoke();
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();

            try
            {
                if (!breakpoint)
                {
                    await PlayAllTurns(_cancellationToken.Token);
                }
                else
                {
                    await HandleBreakpointTurns(_cancellationToken.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Turn execution was cancelled.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error during turn execution: {ex.Message}");
            }

            isPlaying = false;
        }

        private async Task HandleBreakpointTurns(CancellationToken cancellationToken)
        {
            if (!playedTillBreaker)
            {
                await LoadQueue();

                bool playAllTurns = IsBreakerAtExtremes();
                if (playAllTurns)
                {
                    await PlayAllTurns(cancellationToken);
                }
                else
                {
                    await PlayTurnsToBreakpoint(cancellationToken);
                }
            }
            else
            {
                await PlayTurnsFromBreakpoint(cancellationToken);
            }
        }

        private async Task PlayTurnsToBreakpoint(CancellationToken cancellationToken)
        {
            int currentIndex = 0;
            while (currentIndex < breakerIndex && !cancellationToken.IsCancellationRequested)
            {
                await RunNextTurn(cancellationToken);
                currentIndex++;
            }

            playedTillBreaker = true;
            OnPlayedTillBreakpoint?.Invoke();
            turnHolder.BreakpointInitiate(currentIndex);
            breaker.MakeUnInteractable();
        }

        private async Task PlayTurnsFromBreakpoint(CancellationToken cancellationToken)
        {
            int currentIndex = breakerIndex;
            await LoadQueueFromIndex(currentIndex);

            while (turnQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                await RunNextTurn(cancellationToken);
                currentIndex++;
            }

            OnAllTurnsCompleted?.Invoke();
            turnHolder.Reset();
            breaker.MakeInteractable();
        }

        private bool IsBreakerAtExtremes()
        {
            bool isAtExtremes = breakerIndex == 0 || breakerIndex >= turnQueue.Count;
            Debug.Log($"Breaker at extremes: {isAtExtremes}, Queue Count: {turnQueue.Count}");
            return isAtExtremes;
        }

        private async Task PlayAllTurns(CancellationToken cancellationToken)
        {
            await LoadQueue();

            while (turnQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                await RunNextTurn(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                OnAllTurnsCompleted?.Invoke();
            }
        }

        private async Task RunNextTurn(CancellationToken cancellationToken)
        {
            if (turnQueue.Count == 0 || cancellationToken.IsCancellationRequested) return;

            GameTurn turn = turnQueue.Dequeue();

            if (turn.TryGetComponent<UIGameCard>(out var gameCard))
            {
                if (turn.IsAlive())
                {
                    await turn.PerformAsync();

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(MetaConstants.PauseBetweenTurn), cancellationToken);
                        _historyStack.Push(turn);
                        gameCard?.MakeUninteractable();
                        Debug.Log($"Executed: {turn.name}");
                    }
                }
                else
                {
                    Debug.Log($"{turn.name} is dead, turn skipped");
                    gameCard?.MakeUninteractable();
                }
            }
        }

        [ContextMenu("Play Next Turn")]
        public async void PlayNextTurn()
        {
            if (isResetting) return;

            OnTurnsProcessingEvent?.Invoke();

            if (!isQueueLoaded)
            {
                await LoadQueue();
            }

            if (turnQueue.Count > 0)
            {
                await RunNextTurn(CancellationToken.None);
            }
            else
            {
                isQueueLoaded = false;
                OnAllTurnsCompleted?.Invoke();
            }
        }
        #endregion

        #region Reset Methods
        [ContextMenu("Reset Turns")]
        public async void ResetAllTurns()
        {
            if (isPlaying)
            {
                await PerformReset(true);
            }
            else
            {
                await PerformReset(false);
            }
        }

        /// <summary>
        /// Perform reset
        /// </summary>
        /// <param name="isInstant">Bool flag set to true for instant restart</param>
        /// <returns></returns>
        private async Task PerformReset(bool isInstant)
        {
            if (isResetting) return;

            isResetting = true;
            OnTurnsProcessingEvent?.Invoke();

            try
            {
                // Cancel any ongoing operations
                _cancellationToken?.Cancel();
                _resetCancellationToken?.Cancel();
                _resetCancellationToken = new CancellationTokenSource();

                // Immediate cleanup for instant restart
                if (isInstant)
                {
                    await PerformImmediateCleanup();
                }

                // Reset history stack
                await ResetTurnHistory();

                // Reset turn order
                ResetTurnOrder();

                // Reset breaker
                ResetBreaker();

                // Wait a frame to ensure all operations complete
                await Task.Yield();

                // Final cleanup
                await PerformFinalCleanup();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Reset operation was cancelled.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error during reset: {ex.Message}");
            }
            finally
            {
                isResetting = false;
                isQueueLoaded = false;
                OnResetLastTurnCompleted?.Invoke();
            }
        }

        private async Task PerformImmediateCleanup()
        {
            // Force stop all turns
            var queueArray = turnQueue.ToArray();
            foreach (var turn in queueArray)
            {
                turn?.ForceStopTurn();
            }

            // Immediate cleanup of managers
            ProjectileManager.Instance?.ImmediateCleanUp();
            FireSpread.Instance?.ForceInstantRestart();

            await Task.Yield();
        }

        private async Task ResetTurnHistory()
        {
            while (_historyStack.Count > 0)
            {
                if (_resetCancellationToken.Token.IsCancellationRequested) break;

                GameTurn turn = _historyStack.Pop();
                Debug.Log($"Undoing turn for: {turn.name}");

                if (turn.TryGetComponent<UIGameCard>(out var gameCard))
                {
                    gameCard?.MakeInteractable();
                }

                await turn.Undo();
            }
        }

        private void ResetTurnOrder()
        {
            currentTurnOrder.Clear();
            for (int i = 0; i < originalOrder.Count; i++)
            {
                var turn = originalOrder[i];
                if (turn != null)
                {
                    currentTurnOrder.Add(turn);
                    turn.transform.SetSiblingIndex(i);
                }
            }
        }

        private async Task PerformFinalCleanup()
        {
            await Task.Delay(500); // Small delay to ensure all operations complete

            ResetDestroyedEntities();
            LevelObjectiveManager.Instance?.ResetAllObjectives();
            ResetObstacles();
            ResetCharactersToStart();
            ResetTileData();
            ResetChangedTiles();
            ResetBreakpointSystem();
        }

        public void ResetDestroyedEntities()
        {
            for (int i = DestroyedObjects.Count - 1; i >= 0; i--)
            {
                var destroyedObj = DestroyedObjects[i];
                if (destroyedObj == null) continue;

                try
                {
                    if (destroyedObj.CompareTag(MetaConstants.CharacterTag))
                    {
                        var character = destroyedObj.GetComponent<Base_Ch>();
                        if (character != null)
                        {
                            character.EnableObject();
                            character.resetCharState(true);
                            character.UndoAction();
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
                                obstacleData?.EnableObject();

                                var character = destroyedObj.GetComponent<Base_Ch>();
                                if (character != null)
                                {
                                    character.resetCharState(true);
                                    character.UndoAction();
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error resetting destroyed entity {destroyedObj.name}: {ex.Message}");
                }
            }

            DestroyedObjects.Clear();
        }

        public void ResetChangedTiles()
        {
            foreach (var tile in ChangedTiles)
            {
                var gridTile = GridManager.Instance.FindTile(tile.Key);
                Debug.Log($"Changed tile found: {gridTile} with id: {tile.Key}");
                gridTile?.ResetTileToType(tile.Value);
            }
            ChangedTiles.Clear();
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
                breakerIndex = 0;
                breaker.MakeInteractable();
            }
        }
        #endregion

        #region Turn Holder Section
        public void OnCharactersLoaded()
        {
            if (!isResetting)
            {
                turnHolder?.InitializeComplete();
                OnResetLastTurnCompleted?.Invoke();
            }
        }
        #endregion
    }
}