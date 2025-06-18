using System;
using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.LevelSystem;
using Team.UI;
using UnityEngine;

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


        public List<LevelInfoSO> LevelMap = new List<LevelInfoSO>();

        public LevelInfoSO CurrentLevelSO;

        public LevelData CurrentLevelData;

        public Action<LevelInfoSO> OnCurrentLevelUpdated;

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

        public void SetCurrentLevel(LevelInfoSO _level)
        {
            CurrentLevelSO = _level;
          
            OnCurrentLevelUpdated?.Invoke(CurrentLevelSO);
        }

        public void LoadCurrentLevel()
        {
            if (CurrentLevelSO != null)
            {
                if(createdLevel != null)
                {
                    DestroyImmediate(createdLevel.gameObject);
                }
                CurrentLevelData = new LevelData(CurrentLevelSO.Data.LevelName, CurrentLevelSO.Data.State);
                createdLevel = Instantiate(CurrentLevelSO.GameLevelPrefab);
                createdLevel.LoadLevel(); //TODO: Turn this awaitable later 

                StartLevel();
            }
        }

        public void OnCurrentLevelCompleted()
        {
            Debug.Log($"Level {CurrentLevelSO.Data.LevelName} has been completed");

            CurrentLevelData.State = GameConstants.MetaConstants.LevelState.COMPLETED;

            int index = LevelMap.IndexOf(CurrentLevelSO);
            index++;

            var newLevel = LevelMap[index];
            SetCurrentLevel(newLevel);
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
    }
}
