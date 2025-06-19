using System;
using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.LevelSystem;
using Team.UI;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Managers
{
    [DefaultExecutionOrder(-20)]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance = null;

        [Header("Components")]

        [SerializeField]
        private UILevelSelectionScreen selectionScreen;

        [SerializeField]
        private GameLevel createdLevel = null;


        public List<LevelDataSO> LevelList = new List<LevelDataSO>();

        public Dictionary<LevelID, LevelData> LevelMap = new Dictionary<LevelID, LevelData>();

        public LevelData CurrentLevel;

        public Action<LevelData> OnCurrentLevelUpdated;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadLevelMap();
        }

        public void SetCurrentLevel(LevelData _level)
        {
            CurrentLevel = _level;
          
            OnCurrentLevelUpdated?.Invoke(CurrentLevel);
        }

        public void LoadCurrentLevel()
        {
            if (CurrentLevel != null)
            {
                if(createdLevel != null)
                {
                    DestroyImmediate(createdLevel.gameObject);
                }
                createdLevel = Instantiate(CurrentLevel.GameLevelPrefab);
                createdLevel.LoadLevel(); //TODO: Turn this awaitable later 

                StartLevel();
            }
        }

        public void OnCurrentLevelCompleted()
        {
            Debug.Log($"Level {CurrentLevel.Stats.LevelName} has been completed");

            CurrentLevel.Stats.State = LevelState.COMPLETED;

            SetCurrentLevel(GetNextLevel());
        }

        public void PlayNextLevel()
        {

        }


        /// <summary>
        /// This function runs once the level is completed loaded into the game
        /// </summary>
        private void StartLevel()
        {
            UIManager.Instance.ShowGameUI();
        }

        /// <summary>
        /// Fill out the level map dictionary based on all the levels contained 
        /// in the list
        /// </summary>
        private void LoadLevelMap()
        {
            if(LevelList.Count == 0)
            {
                Debug.LogError("The level list is empty", gameObject);
                return;
            }

            foreach(var level in LevelList)
            {
                LevelMap.Add(level.Data.Stats.LevelID, level.Data);
            }
        }

        private LevelData GetNextLevel()
        {
            return LevelMap[CurrentLevel.Stats.NextLevel];
        }
    }
}
