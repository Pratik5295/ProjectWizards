using Team.UI;
using UnityEngine;

namespace Team.Managers
{
    [DefaultExecutionOrder(10)]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance = null;

        private GameTurnManager _turnManager;

        [Header("Components")]

        [SerializeField]
        private ScreenManager _screenManager;

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
            _screenManager = GetComponent<ScreenManager>();

            //Forced to show level selection as the first screen
            ShowLevelSelectionUI();

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

            OnTurnResetCompletedHandler();
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
            restartButton.SetActive(false);
        }

        private void OnTurnsBeingPlayedHandler()
        {
            playButton.SetActive(false);
            restartButton.SetActive(false);
        }

        #region Screen Manager Sections

        public void ShowEmptyUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.EMPTY);
        }

        public void ShowGameUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.GAME);
        }

        public void ShowLevelSelectionUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.LEVEL_SELECT);
        }

        public void ShowPostGameUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.POST_GAME);
        }

        #endregion
    }
}
