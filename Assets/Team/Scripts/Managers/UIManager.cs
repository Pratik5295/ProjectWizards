using UnityEngine;

namespace Team.Managers
{
    [DefaultExecutionOrder(5)]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance = null;

        private GameTurnManager _turnManager;

        [Header("Components")]

        [SerializeField]
        private GameObject playButton;

        [SerializeField]
        private GameObject restartButton;

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
            _turnManager = GameTurnManager.Instance;
            if (_turnManager != null)
            {
                _turnManager.OnTurnsProcessingEvent += OnTurnsBeingPlayedHandler;
                _turnManager.OnAllTurnsCompleted += OnRoundTurnsCompletedHandler;
                _turnManager.OnResetLastTurnCompleted += OnTurnResetCompletedHandler;
            }
            else
            {
                Debug.LogWarning("Turn Manager was not found",gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_turnManager != null)
            {
                _turnManager.OnTurnsProcessingEvent -= OnTurnsBeingPlayedHandler;
                _turnManager.OnAllTurnsCompleted -= OnRoundTurnsCompletedHandler;
                _turnManager.OnResetLastTurnCompleted -= OnTurnResetCompletedHandler;
            }
        }

        private void OnRoundTurnsCompletedHandler()
        {
            playButton.SetActive(false);
            restartButton.SetActive(true);
        }


        private void OnTurnResetCompletedHandler()
        {
            playButton.SetActive(true);
            restartButton.SetActive(true);
        }

        private void OnTurnsBeingPlayedHandler()
        {
            playButton.SetActive(false);
            restartButton.SetActive(false);
        }
    }
}
