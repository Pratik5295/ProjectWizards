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
            UNLOCKED = 1
        }
    }
}

namespace Team.Gameplay.LevelSystem
{
    

    [System.Serializable]
    public class LevelInfoData
    {
        public string LevelName;
        public LevelState State;
        public LevelInfoData Requirements;
        public bool IsCompleted()
        {
            if(Requirements == null)
            {
                return true;
            }

            return Requirements.IsCompleted();
        }
    }


    [CreateAssetMenu(fileName = "LevelInfoSO",menuName = "Team/Data/Levels/Create a Level Info File")]
    public class LevelInfoSO : ScriptableObject
    {
        public LevelInfoData Data;
        public GameLevel GameLevelPrefab;
    }
}
