using Team.Gameplay.GameLevelSystem;
using UnityEngine;
using System.Collections.Generic;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;
using Team.Gameplay.Tutorial;
using FMODUnity;

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
        public string LevelDescription;
        public List<string> LevelObjectives;
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
        public GameObject EnvironmentPrefab;
        public TextAsset DialogueAsset = null;

        public bool HasBreakPoint = false;  //No breaker by default

        public EventReference s_ambience;

        //Place to add tutorial data
        public List<TutorialData> TutorialSteps = new List<TutorialData>();

        public bool HasTutorial => TutorialSteps.Count > 0;

        public LevelData(LevelData _data)
        {
            Stats = new LevelStats(_data.Stats); 
            GameLevelPrefab = _data.GameLevelPrefab;
            EnvironmentPrefab = _data.EnvironmentPrefab;
            DialogueAsset = _data.DialogueAsset;
            HasBreakPoint = _data.HasBreakPoint;
            s_ambience = _data.s_ambience;

            TutorialSteps = new List<TutorialData>();
            foreach (var step in _data.TutorialSteps)
            {
                TutorialSteps.Add(new TutorialData(step)); 
            }

        }
    }


    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "Team/Data/Levels/Create a Level Data File")]
    public class LevelDataSO : ScriptableObject
    {
        public LevelData Data;
    }
}
