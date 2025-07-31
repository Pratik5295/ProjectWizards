using System;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Gameplay.LevelSystem
{
    public class UILevel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI levelNameText;

        public ChapterID ChapterID;

        [SerializeField]
        private LevelData levelData;

        public LevelData Info => levelData;

        public LevelID LevelID => Info.Stats.LevelID;

        [SerializeField]
        private Sprite defaultIcon;

        [SerializeField]
        private Sprite completeIcon;

        [SerializeField]
        private Image levelImage;

        [SerializeField]
        private Button button;  //Temporary will be removed later

        public bool IsCompleted => Status == LevelState.COMPLETED;

        public LevelState Status;

        public Action<LevelID> OnCompletedLevel;

        public void Reset()
        {
            levelImage.sprite = defaultIcon;
        }


        public void PopulateLevelInfo(LevelData _data)
        {
            levelData = _data;

            Status = levelData.Stats.State;    //Status filled based on initial state from SO

            levelNameText.text = levelData.Stats.LevelName;

            ValidateState();
        }

        /// <summary>
        /// This method will listen in future to the changes in the level data
        /// </summary>
        public void ValidateState()
        {
            if (IsCompleted)
            {
                levelImage.sprite = completeIcon;
            }
            else
            {
                levelImage.sprite = defaultIcon;
            }
        }

        public void OnLevelSelected()
        {
            //All levels playable, the locking part happens through chapters
            LevelManager.Instance.SetCurrentLevel(this,levelData.Stats.LevelID);
            LevelManager.Instance.LoadCurrentLevel();
        }

        public void OnLevelCompleted(bool isLoaded = false)
        {
            Status = LevelState.COMPLETED;
            OnCompletedLevel?.Invoke(Info.Stats.LevelID);

            ValidateState();

            if (!isLoaded)
            {
                //Entry point for the Save Manager
                if (SaveManager.Instance != null)
                {
                    //Stuff can be sent
                    SaveManager.Instance.UpdateLevelCompletedOnChapter(ChapterID, LevelID);
                }
            }
        }
    }
}
