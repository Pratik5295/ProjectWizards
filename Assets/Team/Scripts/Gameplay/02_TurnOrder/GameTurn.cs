using System;
using System.Threading.Tasks;
using Team.Gameplay.GridSystem;
using UnityEngine;


namespace Team.Gameplay.TurnSystem
{
    [DefaultExecutionOrder(3)]
    public class GameTurn : MonoBehaviour, IGameMove
    {
        #region Variables
        //public Base_Ch CharacterPrefab;

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
            //if (CharacterPrefab == null)
            //{
            //    Debug.LogError($"Game Turn: {gameObject.name} is missing character");
            //}
            //else
            //{
            //    InitializeCharacter();
            //}
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

        #region Private Methods

        private void InitializeCharacter()
        {
            //characterObject = Instantiate(CharacterPrefab);

            //TileID tileID = new TileID((int)startTileID.x, (int)startTileID.y);
            //var tile = GridManager.Instance.FindTile(tileID);

            //characterObject.transform.position = new Vector3(tile.TilePosition.x, tile.TilePosition.y + 1f, tile.TilePosition.z);
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
