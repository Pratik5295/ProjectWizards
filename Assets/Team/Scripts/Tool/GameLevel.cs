using System.Collections.Generic;
using Team.Data;
using Team.Gameplay.GridSystem;
using Team.Gameplay.ObjectiveSystem;
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
            GridManager.Instance.SetCurrentLevelTile(this);
        }
    }
}
