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
        private GameObject outlineImage;

        [SerializeField]
        private Button button;  //Temporary will be removed later

        public bool IsCompleted => Status == LevelState.COMPLETED;

        public LevelState Status;

        public Action<LevelID> OnCompletedLevel;

        public void Reset()
        {
            levelImage.sprite = defaultIcon;

            UnSelected();
        }

        private void Start()
        {
            UnSelected();
        }


        public void PopulateLevelInfo(LevelData _data)
        {
            levelData = _data;

            Status = levelData.Stats.State;    //Status filled based on initial state from SO

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

        public void UnSelected()
        {
            outlineImage.SetActive(false);
        }

        public void Selected()
        {
            outlineImage.SetActive(true);
        }
    }
}
