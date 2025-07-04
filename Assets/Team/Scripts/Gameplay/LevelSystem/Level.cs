using System;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Gameplay.LevelSystem
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI levelNameText;

        [SerializeField]
        private LevelDataSO levelData;

        public LevelDataSO Info => levelData;

        [SerializeField]
        private Color unlockedColor;

        [SerializeField]
        private Color lockedColor;

        [SerializeField]
        private Image levelImage;

        [SerializeField]
        private Button button;  //Temporary will be removed later

        public bool IsUnlocked => Status != LevelState.LOCKED;

        public LevelState Status;

        public Action<LevelID> OnCompletedLevel;

        private void Start()
        {
           //if(levelData != null)
           // {
           //     PopulateLevelInfo(levelData);
           // }
        }

        public void PopulateLevelInfo(LevelDataSO _data)
        {
            levelData = _data;

            Status = levelData.Data.Stats.State;    //Status filled based on initial state from SO

            levelNameText.text = levelData.Data.Stats.LevelName;

            ValidateState();
        }

        /// <summary>
        /// This method will listen in future to the changes in the level data
        /// </summary>
        public void ValidateState()
        {
            if (IsUnlocked)
            {
                levelImage.color = unlockedColor;
                button.interactable = true;
            }
            else
            {
                levelImage.color = lockedColor;
                button.interactable = false;
            }
        }

        public void OnLevelSelected()
        {
            if (IsUnlocked)
            {
                //Unlocked, allow to play level
                LevelManager.Instance.SetCurrentLevel(levelData.Data.Stats.LevelID);
                LevelManager.Instance.LoadCurrentLevel();
            }
            else
            {
                //Locked
            }
        }

        public void OnLevelCompleted()
        {
            OnCompletedLevel?.Invoke(Info.Data.Stats.LevelID);
        }
    }
}
