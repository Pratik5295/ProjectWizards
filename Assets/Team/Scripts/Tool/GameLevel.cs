using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Team.Data;
using Team.Gameplay.GridSystem;
using Team.Gameplay.ObjectiveSystem;
using Team.Managers;
using UnityEngine;

namespace Team.Gameplay.GameLevelSystem
{
    /// <summary>
    /// This script will be the high level game's level script
    /// holding the reference for:
    /// Tile Creator Object
    /// Character Map for the level
    /// Objectives Map for the level
    /// </summary>
    /// 


    [System.Serializable]
    public class GameLevel : MonoBehaviour
    {
        [Header("Level Tile Prefab")]
        public LevelTileCreator LevelTiles; //This game object will be spawned/instantiated at runtime

        [Header("Characters in the Level")]
        [Tooltip("Load all the characters that would be spawned")]
        public List<CharacterData> CharactersMap = new List<CharacterData>();

        [Header("Objectives in the Level")]
        public List<GameObjectiveData> _objectiveMap = new List<GameObjectiveData>();

        public void LoadLevel()
        {
            GameTurnManager.Instance.EmptyQueue();

            GridManager.Instance.SetCurrentLevelTile(this);

            CharacterManager.Instance.LoadCharactersFromLeveData(CharactersMap);

            LevelObjectiveManager.Instance.LoadObjectivesFromLevelData(_objectiveMap);
        }

        // New async method
        public async UniTask LoadLevelAsync(IProgress<float> progress = null)
        {
            try
            {
                Debug.Log("[GameLevel] Starting async level loading...");

                // Step 1: Empty the turn queue (5% progress)
                Debug.Log("[GameLevel] Step 1: Emptying turn queue...");
                progress?.Report(0.05f);
                GameTurnManager.Instance.EmptyQueue();
                await UniTask.Yield(); // Allow frame to process

                // Step 2: Set current level tile (15% progress)
                Debug.Log("[GameLevel] Step 2: Setting current level tile...");
                progress?.Report(0.15f);
                GridManager.Instance.SetCurrentLevelTile(this);
                await UniTask.Yield();

                // Step 3: Load characters (50% progress from 15% to 65%)
                Debug.Log("[GameLevel] Step 3: Loading characters...");
                await CharacterManager.Instance.LoadCharactersFromLevelDataAsync(
                    CharactersMap,
                    new Progress<float>(p => {
                        float currentProgress = 0.15f + (p * 0.80f);
                        Debug.Log($"[GameLevel] Character loading progress: {p:P1} (Overall: {currentProgress:P1})");
                        progress?.Report(currentProgress);
                    })
                );

                await UniTask.Delay(1000);

                // Step 4: Load objectives (35% progress from 65% to 100%)
                Debug.Log("[GameLevel] Step 4: Loading objectives...");
                await LevelObjectiveManager.Instance.LoadObjectivesFromLevelDataAsync(
                    _objectiveMap,
                    new Progress<float>(p => {
                        float currentProgress = 0.80f + (p * 0.20f);
                        Debug.Log($"[GameLevel] Objective loading progress: {p:P1} (Overall: {currentProgress:P1})");
                        progress?.Report(currentProgress);
                    })
                );

                await UniTask.Yield();

                await UniTask.Delay(1000);

                progress?.Report(1.0f);
                Debug.Log("[GameLevel] Level loading completed successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameLevel] Error loading level: {ex.Message}");
                throw;
            }
        }
        #region Tool Helper Section
        public void LoadChaacterMap(List<CharacterData> _data)
        {
            CharactersMap.Clear();

            foreach (var character in _data)
            {
                CharactersMap.Add(character);   
            }
        }

        public void LoadObjectiveMap(List<GameObjectiveData> _data)
        {
            _objectiveMap.Clear();

            foreach(var objective in _data)
            {
                _objectiveMap.Add(objective);
            }
        }

        #endregion
    }
}
