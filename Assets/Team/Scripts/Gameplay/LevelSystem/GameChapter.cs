using System.Collections.Generic;
using Team.Managers;
using TMPro;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Gameplay.LevelSystem
{
    public class GameChapter : MonoBehaviour
    {
        public ChapterState Status;

        public ChapterDataSO chapterData;

        public List<LevelID> CompletedLevels = new List<LevelID>();

        public List<Level> LevelObjects = new List<Level>();

        public int LevelsCompleted => CompletedLevels.Count;

        [Space(5)]
        [Header("UI Variables/Components")]
        [SerializeField]
        private GameObject uiLevelPrefab;
        [SerializeField]
        private TextMeshProUGUI chapterTitleText;
        [SerializeField]
        private Transform chapterHolderTransform;
        [SerializeField]
        private CanvasGroup chapterCanvasGroup;

        #region Unity Methods

        private void Start()
        {
           
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        #endregion

        #region Level Completion Status

        public void AddCompletedLevel(LevelID levelID)
        {
            if (CompletedLevels.Contains(levelID))
            {
                Debug.LogWarning($"Level: {levelID} has already been completed and added");
                return;
            }

            CompletedLevels.Add(levelID);
        }

        public bool IsLevelCompleted(LevelID _levelID)
        {
            return CompletedLevels.Contains(_levelID);
        }

        #endregion


        #region Level Objects Region

        public void AddLevel(Level level)
        {
            if (LevelObjects.Contains(level)) return;

            LevelObjects.Add(level);

            //Adding listener
            level.OnCompletedLevel += OnLevelCompleted;
        }

        private void UnSubscribeEvents()
        {
            foreach (Level level in LevelObjects)
            {
                level.OnCompletedLevel -= OnLevelCompleted;
            }
        }

        /// <summary>
        /// Connect this listener to the event being fired from the 
        /// </summary>
        /// <param name="_levelID"></param>
        public void OnLevelCompleted(LevelID _levelID)
        {
            AddCompletedLevel(_levelID);
        }

        #endregion

        #region UI Handling Section
        public void Initialize()
        {
            //Sets the initial state
            Status = chapterData.Data.InitialState;

            //Populate self name
            chapterTitleText.text = chapterData.Data.ChapterName;

            //Generate all level objects
            CreateLevelUI();

            //Update canvas group to interactable/uninteractable
            if(Status == ChapterState.LOCKED)
            {
                MakeUnInteractable();
            }
            else
            {
                MakeInteractable();
            }
        }

        public void CreateLevelUI()
        {
            foreach(var levelSO in chapterData.Data.Levels)
            {
                var level = Instantiate(uiLevelPrefab, chapterHolderTransform).GetComponent<Level>();
                level.PopulateLevelInfo(levelSO);

                //Populate Level Manager with Level
                LevelManager.Instance.AddLevelToMap(level);

                //Add local listener to the chapter
                AddLevel(level);
            }
        }

        private void MakeInteractable()
        {
            chapterCanvasGroup.interactable = true;
        }

        private void MakeUnInteractable()
        {
            chapterCanvasGroup.interactable = false;
        }

        #endregion
    }
}
