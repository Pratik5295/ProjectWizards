using System.Collections.Generic;
using Team.Data;
using Team.Gameplay.LevelSystem;
using Team.Gameplay.ObjectiveSystem;
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

        private LevelManager _levelManager;

        [SerializeField]
        private LevelObjectiveManager levelObjectiveManager;

        [Header("Components")]

        [SerializeField]
        private ScreenManager _screenManager;

        [SerializeField]
        private InkDialogueManager _dialogueManager;

        [SerializeField]
        private TutorialManager _tutorialManager;

        [SerializeField]
        private UIGameScreen gameScreen;

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
            levelObjectiveManager = LevelObjectiveManager.Instance;

            _levelManager = LevelManager.Instance;


            //Forced to show level selection as the first screen
            ShowMenuUI();

            if (_turnManager != null)
            {
                _turnManager.OnTurnsProcessingEvent += OnTurnsBeingPlayedHandler;
                _turnManager.OnAllTurnsCompleted += OnRoundTurnsCompletedHandler;
                _turnManager.OnResetLastTurnCompleted += OnTurnResetCompletedHandler;
                _turnManager.OnPlayedTillBreakpoint += OnReachedBreakpointHandler;
                _turnManager.OnRestartProcessingEvent += OnTurnResetStartedHandler;
            }
            else
            {
                Debug.LogWarning("Turn Manager was not found",gameObject);
            }

            if(levelObjectiveManager != null)
            {
                levelObjectiveManager.OnObjectivesCheckCompletedEvent += OnObjectiveCheckCompletedHandler;
            }

            OnTurnResetCompletedHandler();

            //Initialize Tutorial Manager
            _tutorialManager = _tutorialManager.InitializeTutorialsUI(); //Updates itself with the newer instance

            if(_levelManager != null)
            {
                _levelManager.OnCurrentLevelUpdated += OnLevelUpdated;

            }
        }

        private void OnDestroy()
        {
            if (_turnManager != null)
            {
                _turnManager.OnTurnsProcessingEvent -= OnTurnsBeingPlayedHandler;
                _turnManager.OnAllTurnsCompleted -= OnRoundTurnsCompletedHandler;
                _turnManager.OnResetLastTurnCompleted -= OnTurnResetCompletedHandler;
                _turnManager.OnPlayedTillBreakpoint -= OnReachedBreakpointHandler;
                _turnManager.OnRestartProcessingEvent -= OnTurnResetStartedHandler;
            }

            if (levelObjectiveManager != null)
            {
                levelObjectiveManager.OnObjectivesCheckCompletedEvent -= OnObjectiveCheckCompletedHandler;
            }

            if (_levelManager != null)
            {
                _levelManager.OnCurrentLevelUpdated -= OnLevelUpdated;
            }
        }

        private void OnObjectiveCheckCompletedHandler()
        {
            restartButton.SetActive(true);
        }

        private void OnRoundTurnsCompletedHandler()
        {
            playButton.SetActive(false);
            //restartButton.SetActive(true);
        }


        private void OnTurnResetCompletedHandler()
        {
            playButton.SetActive(true);
            restartButton.SetActive(false);

            gameScreen.ShowTurnOrderParent();
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


        private void OnTurnResetStartedHandler()
        {
            gameScreen.HideTurnOrderParent();
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
                _screenManager.GameScreen.ShowTutorialUI();
            }
            else
            {
                _screenManager.GameScreen.HideTutorialUI();
            }
        }

        public void ShowMenuUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.MENU);
        }

        public void ShowChapterSelectionUI()
        {
            _screenManager.ShowScreen(GameConstants.MetaConstants.GameScreen.CHAPTER_SELECT);
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

        public void OnTutorialCompleted()
        {
            _screenManager.GameScreen.HideTutorialUI();
            hasTutorial = false;
        }

        public void OnBackButton()
        {
            //This will take you back to the level selection screen
            LevelManager.Instance.StopLevel();
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

        #region Info Panel Update Section

        public void UpdateInfoPanel(CharacterDataStruct _data)
        {
            gameScreen.PopulateInfoPanel(_data);
        }

        #endregion


        public void UpdateLevelTitleOnObjectives(string _title)
        {
            gameScreen.SetLevelTitleText(_title);
        }

        private void OnLevelUpdated(LevelData _data)
        {
            UpdateLevelTitleOnObjectives(_data.Stats.LevelName);

        }
    }
}
