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
