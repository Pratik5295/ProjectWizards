using System;
using System.Threading.Tasks;
using UnityEngine;


namespace Team.Gameplay.TurnSystem
{
    public class GameTurn : MonoBehaviour, IGameMove
    {
        public GameObject Character; // Could later be ICharacter or similar

        public Action OnTurnStartedEvent;
        public Action OnTurnEndedEvent;

        private TaskCompletionSource<bool> _turnCompletion;

        /// <summary>
        /// Perform the turn and await its completion.
        /// </summary>
        public async Task PerformAsync()
        {
            Debug.Log($"{name} performs turn");

            _turnCompletion = new TaskCompletionSource<bool>();

            OnTurnStartedEvent?.Invoke();

            // Wait until external call completes the turn
            await _turnCompletion.Task;

            OnTurnEndedEvent?.Invoke();

            Debug.Log($"{name} finished turn");
        }

        /// <summary>
        /// Call this when the turn is done (e.g. after animations or player input).
        /// </summary>
        /// 
        [ContextMenu("Complete turn")]
        public void CompleteTurn()
        {
            if (_turnCompletion != null && !_turnCompletion.Task.IsCompleted)
            {
                _turnCompletion.SetResult(true);
            }
        }
    }
}
