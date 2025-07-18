using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.GridSystem;
using Team.Gameplay.LevelSystem;
using Team.UI;
using UnityEngine;
using static Team.GameConstants.LevelConstants;

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
        public GameLevel CreatedLevel
        {
            get { return createdLevel; }
        }

        [SerializeField]
        private GameObject createdEnvironment = null;
        public GameObject CreatedEnvironment
        {
            get { return createdEnvironment; }
        }


        public List<Level> LevelList = new List<Level>();

        public Dictionary<LevelID, Level> LevelMap = new Dictionary<LevelID, Level>();

        public Level CurrentLevel;
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
            //LoadLevelMap();
        }

        public void SetCurrentLevel(LevelID _level)
        {
            CurrentLevelID = _level;

            if(CurrentLevelID != LevelID.NONE)
            {
                //Check if the current level exists in the map
                if (LevelMap.ContainsKey(CurrentLevelID))
                {
                    var original = LevelMap[_level];
                    CurrentLevel = original;


                    //Check if the level has tutorials
                    if (CurrentLevel.Info.Data.HasTutorial)
                    {
                        UIManager.Instance.InitializeTutorial(CurrentLevel.Info.Data.TutorialSteps);
                    }
                    else
                    {
                        UIManager.Instance.ResetNoTutorial();
                    }


                    OnCurrentLevelUpdated?.Invoke(CurrentLevel.Info.Data);
                }
                else
                {
                    Debug.Log($"New current level:{CurrentLevelID} doesnt exist in the level map.");
                }
               
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

                Debug.Log($"[LevelManager] Starting to load level: {CurrentLevel.Info.Data.Stats.LevelName}");

                // Progress tracking with proper logging
                var progressReporter = new Progress<float>(progress =>
                {
                    OnLoadingProgress?.Invoke(progress);
                });

                // Destroy existing level if present
                if (createdLevel != null)
                {
                    DestroyImmediate(createdLevel.gameObject);
                    await UniTask.Yield(); // Allow cleanup to complete
                }

                // Use GameLoadManager to load the level - WAIT for completion
                createdLevel = await gameLoadManager.LoadGameLevelAsync(
                    CurrentLevel.Info.Data.GameLevelPrefab.gameObject,
                    progressReporter
                );


                // Destroy existing environment if not the same.
                if(createdEnvironment != null && createdEnvironment != CurrentLevel.Info.Data.EnvironmentPrefab)
                {
                    DestroyImmediate(createdEnvironment);
                    await UniTask.Yield();
                }

                //Load in Environment.
                createdEnvironment = await gameLoadManager.LoadEnvironmentAsync(
                    CurrentLevel.Info.Data.EnvironmentPrefab,
                    progressReporter
                    );

                // Ensure the level is fully loaded before proceeding
                if (createdLevel == null)
                {
                    throw new Exception("GameLoadManager returned null level!");
                }
                else Debug.Log("CREATED LEVEL!");

                //Spawn Gameplay Managers.
                GridManager.Instance.SpawnGameplayManagers();

                // Setup dialogue if available
                if (CurrentLevel.Info.Data.DialogueAsset != null)
                {
                    UIManager.Instance.SetCurrentDialogue(CurrentLevel.Info.Data.DialogueAsset);
                }

                // Setup turn manager breakpoint
                if (GameTurnManager.Instance != null)
                {
                    GameTurnManager.Instance.HasBreakpoint(CurrentLevel.Info.Data.HasBreakPoint);
                }

                // Wait one more frame to ensure everything is properly initialized
                await UniTask.Yield();

                Debug.Log($"[LevelManager] Level {CurrentLevel.Info.Data.Stats.LevelName} loaded successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LevelManager] Failed to load level: {ex.Message}");
                throw;
            }
            finally
            {
                IsLoading = false;
                OnLoadingCompleted?.Invoke();
                Debug.Log("[LevelManager] Loading process completed!");
            }
        }

        public void OnCurrentLevelCompleted()
        {
            //Notify level its completed
            CurrentLevel.OnLevelCompleted();

            //CurrentLevel.Info.Data.Stats.State = LevelState.COMPLETED;
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
        public void StartLevel()
        {
            if (CurrentLevel.Info.Data.DialogueAsset == null)
            {
                UIManager.Instance.ShowGameUI();
            }
            else
            {
                UIManager.Instance.ShowDialogueUI();
            }
        }

        public void StopLevel()
        {
            UIManager.Instance.ShowLevelSelectionUI();

            CleanupLevelContent();
        }

        public void CleanupLevelContent()
        {
            if (createdLevel != null)
            {
                DestroyImmediate(createdLevel.gameObject);
            }

            if (createdEnvironment != null)
            {
                DestroyImmediate(createdEnvironment.gameObject);
            }
        }

        public void AddLevelToMap(Level _level)
        {
            LevelList.Add(_level);
        }

        /// <summary>
        /// Fill out the level map dictionary based on all the levels contained 
        /// in the list
        /// </summary>
        public void LoadLevelMap()
        {
            if(LevelList.Count == 0)
            {
                Debug.LogError("The level list is empty", gameObject);
                return;
            }

            foreach(var level in LevelList)
            {
                LevelMap.Add(level.Info.Data.Stats.LevelID, level);
            }
        }

        private LevelID GetNextLevel()
        {
            return CurrentLevel.Info.Data.Stats.NextLevel;
        }
    }
}
