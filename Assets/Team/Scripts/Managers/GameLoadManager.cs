using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using System;

namespace Team.Managers
{
    public class GameLoadManager : MonoBehaviour
    {
        [SerializeField] private InstantiateLevelOperation levelOperation;
        [SerializeField] private GameObject defaultEnvironment;
        private IProgress<float> mainProgress;

        public Action OnLoadingStartedEvent;
        public Action OnLoadingFinishedEvent;
        public Action<float,string> OnLoadPercentChangedEvent;

        public async UniTask<Tuple<GameLevel, GameObject>> LoadGameLevelAsync(GameObject _levelPrefab, GameObject _environmentPrefab, IProgress<float> progress = null)
        {
            mainProgress = progress;

            try
            {
                OnLoadingStartedEvent?.Invoke();
                GameInputManager.Instance.canInteract = false;
                Debug.Log("[GameLoadManager] Starting level instantiation...");

                // Step 1: Instantiate the level prefab (30% progress)
                progress?.Report(0.0f);
                levelOperation.SetLevelPrefab(_levelPrefab);
                levelOperation?.SetEnvironmentPrefab(_environmentPrefab);

                var levelGameObject = await levelOperation.LoadAsync(CreateInstantiationProgress());

                var gameLevel = levelGameObject.GetComponent<GameLevel>();
                if (gameLevel == null)
                {
                    throw new Exception("GameLevel component not found on instantiated prefab!");
                }
                if (!_environmentPrefab)
                {
                    if (!defaultEnvironment) { Debug.LogError("NO ENVIRONMENT TO SPAWN! (default or SO environment), please assign one"); }
                    _environmentPrefab = defaultEnvironment;
                    Debug.LogWarning($"NO Environment set on levelData SO! using default instead: {defaultEnvironment.name}");
                }

                Debug.Log("[GameLoadManager] Level prefab instantiated, starting content loading...");

                // Step 2: Load the level content (70% progress from 30% to 100%)
                await gameLevel.LoadLevelAsync(CreateContentLoadingProgress());

                progress?.Report(0.5f);
                Debug.Log("[GameLoadManager] Level loading completed successfully!");

                await UniTask.Yield();
                var EnvironmentInstance = await levelOperation.LoadEnvironmentAsync(CreateInstantiationProgress());
                if (!EnvironmentInstance)
                {
                    Debug.LogError($"[GameLoadManager] Failed to load level - No Environment loaded.");
                    return null;
                }

                await UniTask.Yield();

                Tuple<GameLevel, GameObject> result = new Tuple<GameLevel, GameObject>(gameLevel, EnvironmentInstance);

                progress?.Report(1f);

                OnLoadingFinishedEvent?.Invoke();
                GameInputManager.Instance.canInteract = true;
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameLoadManager] Failed to load level - {ex.Message}");
                throw;
            }
            finally
            {
                mainProgress = null;
            }
        }

        private IProgress<float> CreateInstantiationProgress()
        {
            return new Progress<float>(OnInstantiationProgress);
        }

        private IProgress<float> CreateContentLoadingProgress()
        {
            return new Progress<float>(OnContentLoadingProgress);
        }

        private void OnInstantiationProgress(float instantiationProgress)
        {
            float overallProgress = instantiationProgress * 0.3f; // 0% to 30%
            Debug.Log($"[GameLoadManager] Instantiation progress: {instantiationProgress:P1} (Overall: {overallProgress:P1})");
            string message = $"Instantiation progress: {instantiationProgress:P1} (Overall: {overallProgress:P1})";
            OnLoadPercentChangedEvent?.Invoke(overallProgress, message);
            mainProgress?.Report(overallProgress);
        }

        private void OnContentLoadingProgress(float contentProgress)
        {
            float overallProgress = 0.3f + (contentProgress * 0.7f); // 30% to 100%
            Debug.Log($"[GameLoadManager] Content loading progress: {contentProgress:P1} (Overall: {overallProgress:P1})");
            string message = $"Content loading progress: {contentProgress:P1} (Overall: {overallProgress:P1})";
            OnLoadPercentChangedEvent?.Invoke(overallProgress, message);
            mainProgress?.Report(overallProgress);
        }
    }
}