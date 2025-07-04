using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.Gameplay.ObjectiveSystem;
using Team.Gameplay.TurnSystem;
using Team.GameConstants;
using UnityEngine;
using Team.Gameplay.GridSystem;
using Team.UI.Gameplay;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float PauseBetweenTurn = 0.5f;
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

        public List<GameObject> originalOrder = new List<GameObject>();
        public List<GameObject> currentTurnOrder = new List<GameObject>(); //This will be used to reset the Queue

        [SerializeField]
        private TurnHolder turnHolder;

        [Space(5)]
        [Header("Breakpoint System Variables")]
        [SerializeField]
        private GameBreakpoint breaker; //Reference to the breaker in game turn order
        [SerializeField]
        private int breakerIndex = 0;
        [SerializeField]
        private bool breakpoint = false;    //Breakpoint will be set at runtime by the level
        [SerializeField]
        private bool playedTillBreaker = false;    //Will be set to true after first set till breakpoint is played


        public bool HasCharacterTurns => turnQueue.Count > 0;

        private bool isQueueLoaded = false;

        public Action OnTurnsProcessingEvent;
        public Action OnAllTurnsCompleted;  //TODO: Update this to include the round integer
        public Action OnResetLastTurnCompleted; //TODO: To include which turn count was the round reset to
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

        #endregion

        #region Public Methods

        public void HasBreakpoint(bool _hasBreakPoint)
        {
            breakpoint = _hasBreakPoint;

            breaker.gameObject.SetActive(breakpoint);
        }

        public async Task LoadQueue()
        {
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

            //Wait till the end of frame
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
            //Clear history stack
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
            DestroyedObjects.Add(_destroyedObject);
        }

        public void AddChangedTile(GridTile _changedTile)
        {
            ChangedTiles.Add(_changedTile);
        }

        #endregion

        #region Private Methods

        private void LoadObstacleData()
        {
            Obstacles.Clear();

            var gridManager = GridManager.Instance;
            if (gridManager.Obstacles.Count == 0) return;

            foreach (var obs in gridManager.Obstacles)
            {
                Obstacles.Add(obs);
            }
        }

        private void ResetObstacles()
        {
            foreach (var obs in Obstacles)
            {
                if (obs.TryGetComponent<ObstacleData>(out var obsData))
                {
                    obsData.ResetToStart();
                }
            }
        }
        private void ResetTileData()
        {
            FireSpread.Instance.ResetOilTiles();
        }

        private void ResetBreakpointSystem()
        {
            breakpoint = false;
            playedTillBreaker = false;
            breakerIndex = 0;
        }

        #endregion

        #region Context Menu Methods


        [ContextMenu("Play All Turns")]
        public async void PlayTurns()
        {
            OnTurnsProcessingEvent?.Invoke();

            if (!breakpoint)
            {
                PlayAllTurns();
            }
            else
            {
                //Load the turn order in queue
                if (!playedTillBreaker)
                {
                    //Load the queue
                    await LoadQueue();

                    //1. If breaker is at extremes
                    bool playAllTurns = IsBreakerAtExtremes();
                    if (playAllTurns)
                    {
                        PlayAllTurns();
                    }
                    else
                    {
                        //2. If first section of the game

                        //Check if it is a breakpoint level
                        int currentIndex = 0;

                        while (currentIndex < breakerIndex)
                        {
                            Debug.Log($"Runner index: {currentIndex}");
                            await RunNextTurn();
                            currentIndex++;
                        }

                        playedTillBreaker = true;

                        OnPlayedTillBreakpoint?.Invoke();

                        turnHolder.BreakpointInitiate(currentIndex);

                        breaker.MakeUnInteractable();
                    }
                }
                else
                {
                    //3. Last section of the game 

                    //Check if it is a breakpoint level
                    int currentIndex = breakerIndex;

                    //Redo the queue based on current order
                    await LoadQueueFromIndex(currentIndex);

                    while (turnQueue.Count > 0)
                    {
                        Debug.Log($"Runner index: {currentIndex}");
                        await RunNextTurn();
                        currentIndex++;
                    }

                    Debug.Log("Completed the entire breakpoint system loop?");

                    OnAllTurnsCompleted?.Invoke();

                    turnHolder.Reset();

                    breaker.MakeInteractable();
                }
            }

        }

        private bool IsBreakerAtExtremes()
        {
            //Check if the breakpoint index is at extremes, 0 or last. If yes then ignore it
            bool playAllTurns = breakerIndex == 0 || breakerIndex >= turnQueue.Count; //Turn Queue doesnt contain the breaker
            Debug.Log($"Play Turns is at extreme? {playAllTurns} and turn Queue Count: {turnQueue.Count}");

            return playAllTurns;
        }



        private async void PlayAllTurns()
        {
            //Loads all turns and plays them
            await LoadQueue();

            while (turnQueue.Count > 0)
            {
                await RunNextTurn();
            }

            Debug.Log("All turns completed.");

            OnAllTurnsCompleted?.Invoke();
        }

        private async Task RunNextTurn()
        {
            GameTurn turn = turnQueue.Dequeue();
           if(turn.TryGetComponent<UIGameCard>(out var gameCard))

            if (turn.IsAlive())
            {
                await turn.PerformAsync();

                await Task.Delay(TimeSpan.FromSeconds(MetaConstants.PauseBetweenTurn));

                Debug.Log($"Executing: {turn.name}");

                //Turn was performed by the character, update the stack
                _historyStack.Push(turn);

                //Turn is done, make it uninteractable
                gameCard?.MakeUninteractable();
            }
            else
            {
                Debug.Log($"{turn.name} Move character is dead, turn skipped");

                gameCard?.MakeUninteractable();
            }
        }

        [ContextMenu("Reset Turns")]
        public async void ResetAllTurns()
        {
            OnTurnsProcessingEvent?.Invoke();


            //Reset all moves performed by the characters
            while (_historyStack.Count > 0)
            {
                GameTurn turn = _historyStack.Pop();
                if (turn.TryGetComponent<UIGameCard>(out var gameCard))
                {
                    gameCard?.MakeInteractable();
                }
               
                await turn.Undo();
            }

            //Reset the turn order to original ui order
            currentTurnOrder.Clear();
            for (int i = 0; i < originalOrder.Count; i++)
            {
                var turn = originalOrder[i];
                currentTurnOrder.Add(turn);
                turn.transform.SetSiblingIndex(i);
            }


            ResetBreaker();
            Invoke(nameof(DelayReset), 2f);


        }

        private void DelayReset()
        {

            ResetDestroyedEntities();
            //Set All Objectives to be incomplete
            LevelObjectiveManager.Instance.ResetAllObjectives();

            ResetObstacles();

            //Reset all characters to their saved start position
            ResetCharactersToStart();

            //Reset tile data.
            ResetTileData();

            //Reset tiles that have been changed.
            ResetChangedTiles();

            //Notify that undo was completed
            OnResetLastTurnCompleted?.Invoke();

            isQueueLoaded = false;



            Debug.Log("Completed reset");
        }

        [ContextMenu("Play Next Turn")]
        public async void PlayNextTurn()
        {
            OnTurnsProcessingEvent?.Invoke();

            if (!isQueueLoaded)
            {
                await LoadQueue();
            }

            if (turnQueue.Count > 0)
            {
                GameTurn turn = turnQueue.Dequeue();

                if (turn.IsAlive())
                {
                    await turn.PerformAsync();
                }
                Debug.Log("Current turn has been played");

            }
            else
            {
                Debug.Log("All turns have been played");
                isQueueLoaded = false;

                OnAllTurnsCompleted?.Invoke();
            }



        }

        public void ResetDestroyedEntities()
        {
            for (int i = 0; i < DestroyedObjects.Count; i++)
            {
                if (DestroyedObjects[i].CompareTag(MetaConstants.CharacterTag))
                {
                    DestroyedObjects[i].GetComponent<Base_Ch>().EnableObject();
                    DestroyedObjects[i].GetComponent<Base_Ch>().resetCharState(true);
                    DestroyedObjects[i].GetComponent<Base_Ch>().UndoAction();
                }
                else
                {
                    DestroyedObjects[i].GetComponent<Base_Obstacle>().EnableObject();
                    if (DestroyedObjects[i].GetComponent<MoveableObstacle>())
                    {
                        //DestroyedObjects[i].GetComponent<MoveableObstacle>().resetCharState(true);
                        DestroyedObjects[i].GetComponent<MoveableObstacle>().UndoAction();
                    }
                    else
                    {
                        DestroyedObjects[i].GetComponent<ObstacleData>().EnableObject();
                        if (DestroyedObjects[i].GetComponent<Base_Ch>())
                        {
                            DestroyedObjects[i].GetComponent<Base_Ch>().resetCharState(true);
                            DestroyedObjects[i].GetComponent<Base_Ch>().UndoAction();
                        }
                    }
                }
            }
            DestroyedObjects.Clear();
        }

        public void ResetChangedTiles()
        {
            for (int i = 0; i < ChangedTiles.Count; i++)
            {
                ChangedTiles[i].ResetTypeToDefault();
            }
            ChangedTiles.Clear();
        }

        private void ResetCharactersToStart()
        {
            CharacterManager.Instance.ResetAllCharacters();
        }


        private void ResetBreaker()
        {
            breaker.transform.SetAsFirstSibling();
            breakerIndex = 0;

            breaker.MakeInteractable();
        }

        #endregion

        #region Turn Holder Section

        public void OnCharactersLoaded()
        {
            turnHolder.InitializeComplete();

            //Notify that undo was completed/Or turns have been loaded
            OnResetLastTurnCompleted?.Invoke();
        }

        #endregion
    }
}
