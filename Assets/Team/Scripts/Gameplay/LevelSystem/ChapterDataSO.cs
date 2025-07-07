using UnityEngine;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;
using System.Collections.Generic;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public enum ChapterState
        {
            LOCKED = 0,
            UNLOCKED = 1
        }
    }
}

namespace Team.Gameplay.LevelSystem
{

    [System.Serializable]
    public class ChapterData
    {
        public ChapterID ChapterID;
        public string ChapterName;
        public ChapterState InitialState;
        public List<LevelDataSO> Levels; //Objects will be created out of this

        //Update to throw Chapter Description and stuff over here
    }


    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "Team/Data/Levels/Create a Chapter Data File")]
    public class ChapterDataSO : ScriptableObject
    {
        [Header("Current Chapter Section")]
        public ChapterData Data;

        [Space(5)]
        [Header("Next Chapter Section")]
        public ChapterID NextLevel;
        public int LevelsToCompleteToUnlock;

    }
}
