using System.Collections.Generic;
using System.Linq;
using Team.Managers;
using Team.UI;
using TMPro;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Gameplay.LevelSystem
{
    public class GameChapter : MonoBehaviour
    {
        public ChapterState Status;

        [SerializeField]
        private int levelToCompleteRequirement;

        public ChapterDataSO chapterData;

        public List<LevelID> CompletedLevels = new List<LevelID>();

        public List<UILevel> LevelObjects = new List<UILevel>();

        public int LevelsCompleted => CompletedLevels.Count;

        public ChapterID CurrentChapterID => chapterData.Data.ChapterID;

        public ChapterID NextChapterID => chapterData.NextLevel;

        //[Space(5)]
        //[Header("UI Variables/Components")]
        //[SerializeField]
        //private GameObject uiLevelPrefab;
        //[SerializeField]
        //private TextMeshProUGUI chapterTitleText;
        //[SerializeField]
        //private Transform chapterHolderTransform;
        //[SerializeField]
        //private CanvasGroup chapterCanvasGroup;

        [Space(5)]
        [Header("UI Section")]

        [SerializeField]
        private UIChapter UIChapter;

        #region Unity Methods

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

            //Validation if chapter is completed
            if(LevelsCompleted >= levelToCompleteRequirement)
            {
                ChapterManager.Instance.OnChapterCompleted(this);
            }
        }

        public bool IsLevelCompleted(LevelID _levelID)
        {
            return CompletedLevels.Contains(_levelID);
        }

        #endregion


        #region Level Objects Region

        public void AddLevel(UILevel level)
        {
            if (LevelObjects.Contains(level)) return;

            LevelObjects.Add(level);

            //Adding listener
            level.OnCompletedLevel += OnLevelCompleted;
        }

        private void UnSubscribeEvents()
        {
            foreach (UILevel level in LevelObjects)
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

            //Setting local requirement count
            levelToCompleteRequirement = chapterData.LevelsToCompleteToUnlock;

            //Populate self name

            UIChapterInfo info = new UIChapterInfo(chapterData.Data.ChapterName, chapterData.Data.ChapterSprite, chapterData.Data.ChapterNumberSprite);
            UIChapter.PopulateChapterInfo(info);

            //chapterTitleText.text = chapterData.Data.ChapterName;

            //Generate all level objects
            //CreateLevelUI();
            LoadAllLevels();

            //Update canvas group to interactable/uninteractable
            if (Status == ChapterState.LOCKED)
            {
                MakeUnInteractable();
            }
            else
            {
                MakeInteractable();
            }
        }

        private void LoadAllLevels()
        {
            foreach (var levelSO in chapterData.Data.Levels)
            {
                LevelManager.Instance.AllLevels.Add(new LevelPacket(CurrentChapterID,levelSO.Data));
            }
        }

        public void CreateLevelUI()
        {
            foreach(var levelSO in chapterData.Data.Levels)
            {
                //var level = Instantiate(uiLevelPrefab, chapterHolderTransform).GetComponent<UILevel>();
                //level.PopulateLevelInfo(levelSO);

                ////Populate Level Manager with Level
                //LevelManager.Instance.AddLevelToMap(level);

                ////Add local listener to the chapter
                //AddLevel(level);

                ////Load the level with its relevant chapter id
                //level.ChapterID = CurrentChapterID;
            }
        }

        private void MakeInteractable()
        {
            //chapterCanvasGroup.interactable = true;
        }

        private void MakeUnInteractable()
        {
            //chapterCanvasGroup.interactable = false;
        }

        #endregion

        #region Chapter Update Section

        public void UnlockChapter()
        {
            Status = ChapterState.UNLOCKED;

            MakeInteractable();
        }

        #endregion

        #region Load System Handling Section

        public void OnChapterCompletedLevels(List<LevelID> levelIds)
        {
            //foreach (var id in levelIds)
            //{
            //    var level = LevelObjects.FirstOrDefault(x => x.LevelID == id);
            //    level.OnLevelCompleted(true);
            //}
        }

        #endregion
    }
}
