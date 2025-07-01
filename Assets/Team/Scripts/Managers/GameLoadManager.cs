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

        public async UniTask<GameLevel> LoadGameLevelAsync(GameObject _levelPrefab, IProgress<float> progress = null)
        {
            try
            {
                Debug.Log("[GameLoadManager] Starting level instantiation...");

                // Step 1: Instantiate the level prefab (30% progress)
                progress?.Report(0.0f);
                levelOperation.SetLevelPrefab(_levelPrefab);

                var levelGameObject = await levelOperation.LoadAsync(new Progress<float>(p => {
                    float currentProgress = p * 0.3f;
                    Debug.Log($"[GameLoadManager] Instantiation progress: {p:P1} (Overall: {currentProgress:P1})");
                    progress?.Report(currentProgress);
                }));

                var gameLevel = levelGameObject.GetComponent<GameLevel>();
                if (gameLevel == null)
                {
                    throw new Exception("GameLevel component not found on instantiated prefab!");
                }

                Debug.Log("[GameLoadManager] Level prefab instantiated, starting content loading...");

                // Step 2: Load the level content (70% progress from 30% to 100%)
                await gameLevel.LoadLevelAsync(new Progress<float>(p => {
                    float currentProgress = 0.3f + (p * 0.7f);
                    Debug.Log($"[GameLoadManager] Content loading progress: {p:P1} (Overall: {currentProgress:P1})");
                    progress?.Report(currentProgress);
                }));

                progress?.Report(1.0f);
                Debug.Log("[GameLoadManager] Level loading completed successfully!");

                return gameLevel;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameLoadManager] Failed to load level - {ex.Message}");
                throw;
            }
        }
    }
}
