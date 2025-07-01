using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.LevelSystem;
using Team.UI;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Managers
{
    [DefaultExecutionOrder(-20)]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance = null;

        [Header("Components")]

        [SerializeField]
        private GameLoadManager gameLoadManager;

        [SerializeField]
        private UILevelSelectionScreen selectionScreen;

        [SerializeField]
        private GameLevel createdLevel = null;


        public List<LevelDataSO> LevelList = new List<LevelDataSO>();

        public Dictionary<LevelID, LevelData> LevelMap = new Dictionary<LevelID, LevelData>();

        public LevelData CurrentLevel;
        public LevelID CurrentLevelID;

        public Action<LevelData> OnCurrentLevelUpdated;

        [Header("Loading State")]
        // Loading state tracking
        public bool IsLoading { get; private set; } = false;
        public Action<float> OnLoadingProgress;
        public Action OnLoadingStarted;
        public Action OnLoadingCompleted;

        private void Awake()
        {
            if(Instance == null)
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
            LoadLevelMap();
        }

        public void SetCurrentLevel(LevelID _level)
        {
            CurrentLevelID = _level;

            if(CurrentLevelID != LevelID.NONE)
            {
                var original = LevelMap[_level];
                CurrentLevel = new LevelData(original); // Deep copy here

                OnCurrentLevelUpdated?.Invoke(CurrentLevel);
            }
        }

        public void LoadCurrentLevel()
        {
            LoadCurrentLevelAsync().Forget();
        }

        // New async method
        public async UniTask LoadCurrentLevelAsync()
        {
            if (CurrentLevel == null)
            {
                Debug.LogError("No current level set!");
                return;
            }

            if (IsLoading)
            {
                Debug.LogWarning("Level is already loading!");
                return;
            }

            try
            {
                IsLoading = true;
                OnLoadingStarted?.Invoke();

                Debug.Log($"Starting to load level: {CurrentLevel.Stats.LevelName}");

                // Progress tracking
                var progressReporter = new Progress<float>(progress =>
                {
                    Debug.Log($"Loading Progress: {progress:P1}");
                    OnLoadingProgress?.Invoke(progress);
                });

                // Destroy existing level if present
                if (createdLevel != null)
                {
                    DestroyImmediate(createdLevel.gameObject);
                    await UniTask.Yield(); // Allow cleanup to complete
                }

                // Use GameLoadManager to load the level
                createdLevel = await gameLoadManager.LoadGameLevelAsync(
                    CurrentLevel.GameLevelPrefab.gameObject,
                    progressReporter
                );

                // Setup dialogue if available
                if (CurrentLevel.DialogueAsset != null)
                {
                    UIManager.Instance.SetCurrentDialogue(CurrentLevel.DialogueAsset);
                }

                // Setup turn manager breakpoint
                if (GameTurnManager.Instance != null)
                {
                    GameTurnManager.Instance.HasBreakpoint(CurrentLevel.HasBreakPoint);
                }

                // Start the level
                StartLevel();

                Debug.Log($"Level {CurrentLevel.Stats.LevelName} loaded successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load level: {ex.Message}");
                throw;
            }
            finally
            {
                IsLoading = false;
                OnLoadingCompleted?.Invoke();
            }
        }

        public void OnCurrentLevelCompleted()
        {
            Debug.Log($"Level {CurrentLevel.Stats.LevelName} has been completed");

            CurrentLevel.Stats.State = LevelState.COMPLETED;
            SetCurrentLevel(GetNextLevel());
        }

        public void PlayNextLevel()
        {
            if (CurrentLevelID == LevelID.NONE)
            {
                Debug.Log("No next level to play");
            }
            else
            {
                LoadCurrentLevel();
            }
        }


        /// <summary>
        /// This function runs once the level is completed loaded into the game
        /// </summary>
        private void StartLevel()
        {
            if (CurrentLevel.DialogueAsset == null)
            {
                UIManager.Instance.ShowGameUI();
            }
            else
            {
                UIManager.Instance.ShowDialogueUI();
            }
        }

        /// <summary>
        /// Fill out the level map dictionary based on all the levels contained 
        /// in the list
        /// </summary>
        private void LoadLevelMap()
        {
            if(LevelList.Count == 0)
            {
                Debug.LogError("The level list is empty", gameObject);
                return;
            }

            foreach(var level in LevelList)
            {
                LevelMap.Add(level.Data.Stats.LevelID, level.Data);
            }
        }

        private LevelID GetNextLevel()
        {
            return CurrentLevel.Stats.NextLevel;
        }
    }
}
