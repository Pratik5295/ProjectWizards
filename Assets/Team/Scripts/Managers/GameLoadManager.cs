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
                Debug.Log("GameLoadManager: Starting level instantiation...");

                // Step 1: Instantiate the level prefab (30% progress)
                progress?.Report(0.0f);
                levelOperation.SetLevelPrefab(_levelPrefab);

                var levelGameObject = await levelOperation.LoadAsync(new Progress<float>(p =>
                    progress?.Report(p * 0.3f)
                ));

                var gameLevel = levelGameObject.GetComponent<GameLevel>();
                if (gameLevel == null)
                {
                    throw new Exception("GameLevel component not found on instantiated prefab!");
                }

                // Step 2: Load the level content (70% progress)
                progress?.Report(0.3f);
                await gameLevel.LoadLevelAsync(new Progress<float>(p =>
                    progress?.Report(0.3f + (p * 0.7f))
                ));

                progress?.Report(1.0f);
                Debug.Log("GameLoadManager: Level loading completed successfully!");

                return gameLevel;
            }
            catch (Exception ex)
            {
                Debug.LogError($"GameLoadManager: Failed to load level - {ex.Message}");
                throw;
            }
        }
    }
}
