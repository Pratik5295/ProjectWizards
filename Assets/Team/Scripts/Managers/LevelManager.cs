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
        public GameLevel CreatedLevel
        {
            get { return createdLevel; }
        }


        public List<LevelDataSO> LevelList = new List<LevelDataSO>();

        public Dictionary<LevelID, LevelData> LevelMap = new Dictionary<LevelID, LevelData>();

        public LevelData CurrentLevel;
        public LevelID CurrentLevelID;

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

        public void SetCurrentLevel(LevelID _level)
        {
            CurrentLevelID = _level;

            if(CurrentLevelID != LevelID.NONE)
            {
                var original = LevelMap[_level];
                CurrentLevel = new LevelData(original); // Deep copy here

                OnCurrentLevelUpdated?.Invoke(CurrentLevel);
            }
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

                //Load the dialogue manager with this
                if(CurrentLevel.DialogueAsset != null)
                    UIManager.Instance.SetCurrentDialogue(CurrentLevel.DialogueAsset);  

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
            if (CurrentLevelID == LevelID.NONE)
            {
                Debug.Log("No next level to play");
            }
            else
            {
                LoadCurrentLevel();
            }
        }


        /// <summary>
        /// This function runs once the level is completed loaded into the game
        /// </summary>
        private void StartLevel()
        {
            if (CurrentLevel.DialogueAsset == null)
            {
                UIManager.Instance.ShowGameUI();
            }
            else
            {
                UIManager.Instance.ShowDialogueUI();
            }
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

        private LevelID GetNextLevel()
        {
            return CurrentLevel.Stats.NextLevel;
        }
    }
}
