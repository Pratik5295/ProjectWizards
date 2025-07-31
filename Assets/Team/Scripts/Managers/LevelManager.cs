using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.GridSystem;
using Team.Gameplay.LevelSystem;
using Team.Gameplay.ObjectiveSystem;
using Team.UI;
using UnityEngine;
using static Team.GameConstants.LevelConstants;

namespace Team.Managers
{
    [System.Serializable]
    public struct LevelPacket
    {
        public ChapterID chapterID;
        public LevelData LevelData;

        public LevelPacket(ChapterID _chapterId, LevelData _levelData)
        {
            chapterID = _chapterId;
            LevelData = _levelData;
        }
    }

    [DefaultExecutionOrder(-20)]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance = null;

        [Header("Components")]

        [SerializeField]
        private GameLoadManager gameLoadManager;

        [SerializeField]
        private UILevelSelectionScreen levelSelectionScreen;

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

        public List<LevelPacket> AllLevels = new List<LevelPacket>();

        public Dictionary<LevelID, LevelData> LevelMap = new Dictionary<LevelID, LevelData>();

        public UILevel CurrentLevel;
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
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetCurrentLevel(UILevel _uiLevel,LevelID _level)
        {
            CurrentLevelID = _level;

            if (CurrentLevelID != LevelID.NONE)
            {
                //Check if the current level exists in the map
                if (LevelMap.ContainsKey(CurrentLevelID))
                {
                    CurrentLevel = _uiLevel;


                    //Check if the level has tutorials
                    if (CurrentLevel.Info.HasTutorial)
                    {
                        UIManager.Instance.InitializeTutorial(CurrentLevel.Info.TutorialSteps);
                    }
                    else
                    {
                        UIManager.Instance.ResetNoTutorial();
                    }

                    levelSelectionScreen.OnSetLevelInfoPanel(CurrentLevel.Info);

                    OnCurrentLevelUpdated?.Invoke(CurrentLevel.Info);
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

                // Destroy existing environment if not the same.
                if (createdEnvironment != null)
                {
                    DestroyImmediate(createdEnvironment);
                    await UniTask.Yield();
                }


                Tuple<GameLevel, GameObject> result = null;
                // Use GameLoadManager to load the level - WAIT for completion
                result = await gameLoadManager.LoadGameLevelAsync(
                    CurrentLevel.Info.GameLevelPrefab.gameObject, CurrentLevel.Info.EnvironmentPrefab,
                    progressReporter
                );

                createdLevel = result.Item1;
                createdEnvironment = result.Item2;

                //Load in Environment.
                /* createdEnvironment = await gameLoadManager.LoadEnvironmentAsync(
                     CurrentLevel.Info.Data.EnvironmentPrefab,
                     progressReporter
                     );*/

                // Ensure the level is fully loaded before proceeding
                if (createdLevel == null)
                {
                    throw new Exception("GameLoadManager returned null level!");
                }
                else Debug.Log("CREATED LEVEL!");

                //Spawn Gameplay Managers.
                GridManager.Instance.SpawnGameplayManagers();
                FireSpread.Instance.Initialise();

                // Setup dialogue if available
                if (CurrentLevel.Info.DialogueAsset != null)
                {
                    UIManager.Instance.SetCurrentDialogue(CurrentLevel.Info.DialogueAsset);
                }

                // Setup turn manager breakpoint
                if (GameTurnManager.Instance != null)
                {
                    GameTurnManager.Instance.HasBreakpoint(CurrentLevel.Info.HasBreakPoint);
                }

                // Wait one more frame to ensure everything is properly initialized
                await UniTask.Yield();
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
            if (EnvrionmentalsManager.Instance)
            {
                EnvrionmentalsManager.Instance.isRunning = false;
            }
            //Notify level its completed
            CurrentLevel.OnLevelCompleted();


            var nextLevelID = GetNextLevel();
            var levelUI = levelSelectionScreen.GetLevelBox(nextLevelID);

            //CurrentLevel.Info.Data.Stats.State = LevelState.COMPLETED;
            SetCurrentLevel(levelUI,nextLevelID);
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
            if (CurrentLevel.Info.DialogueAsset == null)
            {
                UIManager.Instance.ShowGameUI();
            }
            else
            {
                UIManager.Instance.ShowDialogueUI();
            }
        }

        public async void StopLevel()
        {
            UIManager.Instance.ShowLevelSelectionUI();

            await CleanupLevelContentAsync();
        }

        public async Task CleanupLevelContentAsync()
        {
            // 1. Destroy Level
            if (createdLevel != null)
            {
                DestroyImmediate(createdLevel.gameObject);
                await Task.Yield(); // Give up control to allow frame update
            }

            // 2. Destroy Environment
            if (createdEnvironment != null)
            {
                DestroyImmediate(createdEnvironment.gameObject);
                await Task.Yield();
            }

            // 3. Clean up Characters & Game Cards
            if (CharacterManager.Instance != null)
            {
                CharacterManager.Instance.CleanUp();
                await Task.Yield();
            }

            // 4. Clean up Objectives
            if (LevelObjectiveManager.Instance != null)
            {
                LevelObjectiveManager.Instance.CleanUp();
                await Task.Yield();
            }

            // 5. Clean up Projectiles
            if (ProjectileManager.Instance != null)
            {
                ProjectileManager.Instance.ImmediateCleanUp();
                await Task.Yield();
            }

            // 6. Clean up FireSpread
            if (FireSpread.Instance != null)
            {
                FireSpread.Instance.CleanUp();
                await Task.Yield();
            }

            // 7. Clean up Turn Manager
            if (GameTurnManager.Instance != null)
            {
                GameTurnManager.Instance.CleanUpLevel();
                await Task.Yield();
            }
        }


        /// <summary>
        /// Fill out the level map dictionary based on all the levels contained 
        /// in the list
        /// </summary>
        public void LoadLevelMap()
        {
            if (AllLevels.Count == 0)
            {
                Debug.LogError("The level list is empty", gameObject);
                return;
            }

            foreach (var level in AllLevels)
            {
                var levelID = level.LevelData.Stats.LevelID;
                LevelMap.Add(levelID, level.LevelData);
            }
        }

        private LevelID GetNextLevel()
        {
            return CurrentLevel.Info.Stats.NextLevel;
        }
    }
}
