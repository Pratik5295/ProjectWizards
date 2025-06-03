using System;
using System.Threading.Tasks;
using Team.Gameplay.GridSystem;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace Team.Gameplay.TurnSystem
{
    [DefaultExecutionOrder(3)]
    public class GameTurn : MonoBehaviour, IGameMove
    {
        #region Variables

        [SerializeField]
        private Base_Ch characterObject;

        [SerializeField]
        private Vector2 startTileID;  //TODO: later it will come directly from SO

        [SerializeField]
        private float duration; //Only for testing

        public Action OnTurnStartedEvent;
        public Action OnTurnEndedEvent;

        private TaskCompletionSource<bool> _turnCompletion;
        #endregion

        #region Unity Methods

        private void Start()
        {

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Prior to playing the move, the game checks if the said character is alive to perform the turn
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return characterObject.IsAlive;
        }

        /// <summary>
        /// Perform the turn and await its completion.
        /// </summary>
        public async Task PerformAsync()
        {
            Debug.Log($"{name} performs turn");

            _turnCompletion = new TaskCompletionSource<bool>();

            OnTurnStartedEvent?.Invoke();

            characterObject.UseAbility();

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

        public void SetupGameTurn(Base_Ch _character)
        {
            characterObject = _character;
            characterObject.OnTurnComplete += CompleteTurn;
        }

        #endregion

        #region Private Methods


        #endregion
    }
}
