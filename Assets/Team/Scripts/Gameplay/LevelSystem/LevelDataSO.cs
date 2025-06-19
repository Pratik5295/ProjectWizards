using Team.Gameplay.GameLevelSystem;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
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
    public class LevelStats
    {
        public LevelID LevelID;
        public string LevelName;
        public LevelState State;
        public LevelID NextLevel;

        public LevelStats(LevelID _levelID,string _levelName,  LevelState _state, LevelID nextLevel)
        {
            LevelID = _levelID;
            LevelName = _levelName;
            State = _state;
            NextLevel = nextLevel;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public LevelStats Stats;
        public GameLevel GameLevelPrefab;
    }


    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "Team/Data/Levels/Create a Level Data File")]
    public class LevelDataSO : ScriptableObject
    {
        public LevelData Data;
    }
}
