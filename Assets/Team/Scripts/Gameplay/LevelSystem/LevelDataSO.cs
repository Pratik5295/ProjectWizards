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

        public LevelStats(LevelStats other)
        {
            LevelID = other.LevelID;
            State = other.State;
            NextLevel = other.NextLevel;
            LevelName = other.LevelName;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public LevelStats Stats;
        public GameLevel GameLevelPrefab;
        public TextAsset DialogueAsset = null;

        public LevelData(LevelData _data)
        {
            Stats = new LevelStats(_data.Stats); // Also a deep copy
            GameLevelPrefab = _data.GameLevelPrefab;
            DialogueAsset = _data.DialogueAsset;
        }
    }


    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "Team/Data/Levels/Create a Level Data File")]
    public class LevelDataSO : ScriptableObject
    {
        public LevelData Data;
    }
}
