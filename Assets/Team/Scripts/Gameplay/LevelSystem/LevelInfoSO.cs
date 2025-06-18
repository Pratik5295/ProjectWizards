using Team.Gameplay.GameLevelSystem;
using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public enum LevelState
        {
            LOCKED = 0,
            UNLOCKED = 1,
            COMPLETED = 2
        }
    }
}

namespace Team.Gameplay.LevelSystem
{
    

    [System.Serializable]
    public class LevelData
    {
        public string LevelName;
        public LevelState State;

        public LevelData(string _levelName,  LevelState _state)
        {
            LevelName = _levelName;
            State = _state;
        }
    }


    [CreateAssetMenu(fileName = "LevelInfoSO",menuName = "Team/Data/Levels/Create a Level Data File")]
    public class LevelInfoSO : ScriptableObject
    {
        public LevelData Data;
        public GameLevel GameLevelPrefab;
    }
}
