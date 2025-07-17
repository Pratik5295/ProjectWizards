using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Team.Data;
using Team.GameConstants;
using Team.Gameplay.Characters;
using Team.Managers;
using Team.UI;
using UnityEngine;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float ShowPostGameScreenAfter = 2f;
    }
}

namespace Team.Gameplay.ObjectiveSystem
{
    //Make sure it runs after Game Turn Manager
    [DefaultExecutionOrder(3)]
    public class LevelObjectiveManager : MonoBehaviour, ILoadingOperation
    {
        public static LevelObjectiveManager Instance = null;

        [SerializeField] private List<GameObjectiveData> _objectiveMap = new List<GameObjectiveData>();
        [Header("Private Local cache, dont fill")]
        [SerializeField] private List<GenericObjective> _levelObjectives = new List<GenericObjective>();
        [Header("Components")]
        [SerializeField] private GameTurnManager turnManager;
        [SerializeField] private UIObjectivesHolder objectivesHolder;

        public string Description => "Loading Objectives...";

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
            turnManager = GameTurnManager.Instance;
            if (turnManager == null)
            {
                Debug.LogWarning("Game Turn Manager not found");
            }
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        // Original synchronous method for backwards compatibility
        public void LoadObjectivesFromLevelData(List<GameObjectiveData> _objectives)
        {
            LoadObjectivesFromLevelDataAsync(_objectives).Forget();
        }

        // New async method
        public async UniTask LoadObjectivesFromLevelDataAsync(List<GameObjectiveData> _objectives, IProgress<float> progress = null)
        {
            try
            {
                Debug.Log("[LevelObjectiveManager] Starting objectives loading...");

                CleanUp();
                _objectiveMap.Clear();

                // Copy objective data (20% progress)
                progress?.Report(0.2f);
                foreach (var objective in _objectives)
                {
                    _objectiveMap.Add(objective);
                }

                // Initialize objectives with progress tracking (80% progress)
                await InitializeObjectivesAsync(new Progress<float>(p => {
                    float currentProgress = 0.2f + (p * 0.8f);
                    progress?.Report(currentProgress);
                }));

                Debug.Log("[LevelObjectiveManager] Objectives loading completed!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LevelObjectiveManager] Error loading objectives: {ex.Message}");
                throw;
            }
        }

        // Original synchronous method for backwards compatibility
        public void InitalizeObjectives()
        {
            InitializeObjectivesAsync().Forget();
        }

        // New async method
        public async UniTask InitializeObjectivesAsync(IProgress<float> progress = null)
        {
            CharacterManager characterManager = CharacterManager.Instance;
            int totalObjectives = _objectiveMap.Count;
            Debug.Log($"[LevelObjectiveManager] Initializing {totalObjectives} objectives...");

            for (int i = 0; i < totalObjectives; i++)
            {
                var data = _objectiveMap[i];
                Debug.Log($"[LevelObjectiveManager] Creating objective: {data.ObjectiveName} ({i + 1}/{totalObjectives})");

                var objective = ObjectiveFactory.CreateObjective(data);

                foreach (var objectTarget in data.ObjectiveTargets)
                {
                    var characterObject = characterManager.GetCharacter(objectTarget);
                    if (characterObject == null)
                    {
                        Debug.LogError($"Could not find character target for objective: {data.ObjectiveName}", gameObject);
                        continue;
                    }

                    var priority = data.Priority;
                    var skinner = characterObject.GetComponent<CharacterReskinner>();
                    if(skinner == null)
                    {
                        Debug.LogError($"Character skinner component is missing for: {characterObject.name}");
                    }

                    skinner.SetTargetObjective(priority);
                    objective.AddCharacterReference(characterObject);
                }

                _levelObjectives.Add(objective);

                objectivesHolder.SetLevelTitle("Disciples of the Garden");
                objectivesHolder.AddObjective(data);

                // Report progress
                float objectiveProgress = (float)(i + 1) / totalObjectives;
                progress?.Report(objectiveProgress);

                // Yield control to prevent frame drops
                await UniTask.Yield();
            }

            Debug.Log("[LevelObjectiveManager] All objectives initialized!");
        }

        // ILoadingOperation implementation
        public async UniTask LoadAsync(IProgress<float> progress = null)
        {
            await LoadObjectivesFromLevelDataAsync(_objectiveMap, progress);
        }

        public void CleanUp()
        {
            objectivesHolder.ClearAllObjectives();
        }

        public void ResetAllObjectives()
        {
            foreach (var objective in _levelObjectives)
            {
                objective.ResetObjective();
                objectivesHolder.UpdateObjective(objective.Data, false);
            }
        }

        private void RegisterEvents()
        {
            if (turnManager != null)
            {
                turnManager.OnAllTurnsCompleted += OnRoundTurnsCompletedHandler;
            }
        }

        private void UnregisterEvents()
        {
            if (turnManager != null)
            {
                turnManager.OnAllTurnsCompleted -= OnRoundTurnsCompletedHandler;
            }
        }

        private void OnRoundTurnsCompletedHandler()
        {
            if (_levelObjectives.Count == 0)
            {
                Debug.LogWarning("There are no objectives for this level?");
                return;
            }

            bool levelCompleted = true;

            foreach (var objective in _levelObjectives)
            {
                bool result = objective.CheckObjectiveComplete();
                if (!result && objective.Data.Priority == ObjectivePriority.PRIMARY)   //Primary Objectives failing would result in level failure
                {
                    levelCompleted = false;
                }
                objectivesHolder.UpdateObjective(objective.Data, result);
            }

            if (levelCompleted)
            {
                LevelManager.Instance.OnCurrentLevelCompleted();
                UIManager.Instance.ShowEmptyUI();
                Invoke(nameof(ShowLevelCompletedUI), MetaConstants.ShowPostGameScreenAfter);
            }
        }

        private void ShowLevelCompletedUI()
        {
            UIManager.Instance.ShowPostGameUI();

            LevelManager.Instance.StopLevel();
        }
    }
}
