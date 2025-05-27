using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace Team.Gameplay.TurnSystem
{
    public class GameTurn : MonoBehaviour, IGameMove
    {
        #region Variables
        public Base_Ch Character; // Could later be Base Character

        [SerializeField]
        private float duration; //Only for testing

        public Action OnTurnStartedEvent;
        public Action OnTurnEndedEvent;

        private TaskCompletionSource<bool> _turnCompletion;
        #endregion

        #region Unity Methods

        private void Start()
        {
            if(Character == null)
            {
                Debug.LogError($"Game Turn: {gameObject.name} is missing character");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Prior to playing the move, the game checks if the said character is alive to perform the turn
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return Character.IsAlive;
        }

        /// <summary>
        /// Perform the turn and await its completion.
        /// </summary>
        public async Task PerformAsync()
        {
            Debug.Log($"{name} performs turn");

            _turnCompletion = new TaskCompletionSource<bool>();

            OnTurnStartedEvent?.Invoke();

            Character.UseAbility();

            //Only for testing
            Invoke("TestingCompleteTurn",duration);

            // Wait until external call completes the turn
            await _turnCompletion.Task;

            OnTurnEndedEvent?.Invoke();

            Debug.Log($"{name} finished turn");
        }

        /// <summary>
        /// Call this when the turn is done (e.g. after animations or player input).
        /// </summary>
        public void CompleteTurn()
        {
            if (_turnCompletion != null && !_turnCompletion.Task.IsCompleted)
            {
                _turnCompletion.SetResult(true);
            }
        }

        #endregion

        /// <summary>
        /// TESTING: TO BE REMOVED
        /// </summary>
        private void TestingCompleteTurn()
        {
            CompleteTurn();
        }
    }
}
