using System.Collections.Generic;
using Team.Gameplay.Tutorial;
using Team.UI;
using Team.UI.DialogueSystem;
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
        private InkDialogueManager _dialogueManager;

        [SerializeField]
        private TutorialManager _tutorialManager;

        [Space(5)]
        [SerializeField]
        private GameObject playButton;

        [SerializeField]
        private GameObject restartButton;

        [SerializeField]
        private bool hasTutorial = false;

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
                _turnManager.OnPlayedTillBreakpoint += OnReachedBreakpointHandler;
            }
            else
            {
                Debug.LogWarning("Turn Manager was not found",gameObject);
            }

            OnTurnResetCompletedHandler();

            //Initialize Tutorial Manager
            _tutorialManager = _tutorialManager.InitializeTutorialsUI(); //Updates itself with the newer instance
        }

        private void OnDestroy()
        {
            if (_turnManager != null)
            {
                _turnManager.OnTurnsProcessingEvent -= OnTurnsBeingPlayedHandler;
                _turnManager.OnAllTurnsCompleted -= OnRoundTurnsCompletedHandler;
                _turnManager.OnResetLastTurnCompleted -= OnTurnResetCompletedHandler;
                _turnManager.OnPlayedTillBreakpoint -= OnReachedBreakpointHandler;
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

        private void OnReachedBreakpointHandler()
        {
            playButton.SetActive(true);
            restartButton.SetActive(true);
        }

        #region Screen Manager Sections

        public void ShowEmptyUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.EMPTY);
        }

        public void ShowDialogueUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.DIALOGUE);
        }

        public void ShowGameUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.GAME);

            if (hasTutorial)
            {
                Debug.Log("Pratik Showing Tut");
                _screenManager.GameScreen.ShowTutorialUI();
            }
            else
            {
                Debug.Log("Pratik No Tut");
                _screenManager.GameScreen.HideTutorialUI();
            }
        }


        public void ShowLevelSelectionUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.LEVEL_SELECT);
        }

        public void ShowPostGameUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.POST_GAME);
        }

        public void ShowLoadingScreen()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.LOADING);
        }

        #endregion

        #region Dialogue Manager Handling

        public void SetCurrentDialogue(TextAsset _asset)
        {
            _dialogueManager.SetDialogue(_asset);
        }

        #endregion

        #region Tutorial Handling Section

        public void InitializeTutorial(List<TutorialData> _tutorialList)
        {
            Debug.Log($"Yes there are tutorials: {_tutorialList.Count}");
            _tutorialManager.LoadTutorialSteps( _tutorialList);

            hasTutorial = true;
        }

        public void ResetNoTutorial()
        {
            Debug.Log("No tutorial for this level");

            _tutorialManager.ClearTutorialSteps();
            hasTutorial = false;
        }

        #endregion
    }
}
