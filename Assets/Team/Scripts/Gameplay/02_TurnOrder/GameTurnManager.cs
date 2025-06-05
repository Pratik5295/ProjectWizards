using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.Gameplay.ObjectiveSystem;
using Team.Gameplay.TurnSystem;
using UnityEngine;

namespace Team.MetaConstants
{
    public static partial class MetaConstants
    {
        public const float PauseBetweenTurn = 2f;
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

        public List<GameObject> originalOrder = new List<GameObject>();
        public List<GameObject> currentTurnOrder = new List<GameObject>(); //This will be used to reset the Queue

        [SerializeField]
        private Transform turnHolder;

        public bool HasCharacterTurns => turnQueue.Count > 0;

        public Action OnTurnsProcessingEvent;
        public Action OnAllTurnsCompleted;  //TODO: Update this to include the round integer
        public Action OnResetLastTurnCompleted; //TODO: To include which turn count was the round reset to

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

        private void Start()
        {
        }

        #endregion

        #region Public Methods

        public async Task LoadQueue()
        {
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
                turnQueue.Enqueue(unit.GetComponent<GameTurn>());
            }

        }

        public void ForceRebuildTurns()
        {
            if (turnHolder.childCount == 0)
            {
                Debug.LogError("Character turns are missing");
                return;
            }

            currentTurnOrder.Clear();
            for (int i = 0; i < turnHolder.childCount; i++)
            {
                currentTurnOrder.Add(turnHolder.GetChild(i).gameObject);
            }
        }

        public void AddCharacterToTurnOrder(GameObject _turnObject)
        {
            if (originalOrder.Contains(_turnObject)) return;
            originalOrder.Add(_turnObject);

            if(currentTurnOrder.Contains(_turnObject)) return;
            currentTurnOrder.Add(_turnObject);
        }

        #endregion


        #region Context Menu Methods

        [ContextMenu("Play All Turns")]
        public async void PlayTurns()
        {
            OnTurnsProcessingEvent?.Invoke();

            await LoadQueue();

            while (turnQueue.Count > 0)
            {
                GameTurn turn = turnQueue.Dequeue();

                if (turn.IsAlive())
                {
                    await turn.PerformAsync();

                    await Task.Delay(TimeSpan.FromSeconds(MetaConstants.PauseBetweenTurn));

                    //Turn was performed by the character, update the stack
                    _historyStack.Push(turn);
                }
                else
                {
                    Debug.Log($"{turn.name} Move character is dead, turn skipped");
                }
            }

            Debug.Log("All turns completed.");

            OnAllTurnsCompleted?.Invoke();
        }

        [ContextMenu("Reset Turns")]
        public async void ResetAllTurns()
        {
            OnTurnsProcessingEvent?.Invoke();
            //Reset all moves performed by the characters
            while (_historyStack.Count > 0)
            {
                GameTurn turn = _historyStack.Pop();
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

            //Set All Objectives to be incomplete
            LevelObjectiveManager.Instance.ResetAllObjectives();

            //Notify that undo was completed
            OnResetLastTurnCompleted?.Invoke();

        }

        #endregion
    }

}
