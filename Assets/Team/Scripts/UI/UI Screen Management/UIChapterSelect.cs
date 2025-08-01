using Team.Managers;
using UnityEngine;
using System.Collections.Generic;
using Team.Gameplay.LevelSystem;
using DG.Tweening;
using static Team.GameConstants.LevelConstants;

namespace Team.UI
{
    public class UIChapterSelect : UIScreen
    {
        public int currentChapterIndex = 0;
        public int maxChapterIndex;

        public List<ChapterDataSO> ChapterInfo = new List<ChapterDataSO>();

        public List<ChapterData> ChapterDataList = new List<ChapterData>();

        //For now let it be one screen UI
        [SerializeField]
        private UIChapter UIChapter;

        [SerializeField]
        private UILevelSelectionScreen levelSelectionScreen;

        [SerializeField] private RectTransform chapterContainer;
        [SerializeField] private float elementSpacing = 1100f; // space between items (1550 - 450 = 1100)
        [SerializeField] private float animationDuration = 0.5f;


        public override void Start()
        {
            base.Start();

            OnHide();

            //Build the chapters data file

            foreach (var chapter in ChapterInfo)
            {
                ChapterDataList.Add(chapter.Data);
            }

            maxChapterIndex = ChapterDataList.Count - 1;

            ShowCurrentChapterData();
        }

        public void GoToNextChapter()
        {
            if(currentChapterIndex < maxChapterIndex)
            {
                currentChapterIndex++;

                //Show whatever next chapter is
                ShowCurrentChapterData();
            }
            else
            {
                Debug.Log("Last element shown");
            }
        }

        public void GoToPreviousChapter()
        {
            if(currentChapterIndex > 0)
            {
                currentChapterIndex--;

                //Show whatever previous chapter is

                ShowCurrentChapterData();
            }
            else
            {
                Debug.Log("First element shown");
            }
        }

        private void ShowCurrentChapterData()
        {
            // Calculate target anchored position for the container
            float targetX = -currentChapterIndex * elementSpacing;

            // Animate the container to move to the correct position
            chapterContainer.DOAnchorPosX(targetX, animationDuration).SetEase(Ease.InOutCubic);
        }

        public void GoBackToMainMenu()
        {
            UIManager.Instance.ShowMenuUI();

            ResetChapterScroll(true);
        }

        public void OnSelectButtonClicked()
        {
            //Set The Current selected chapter
            ChapterID currentChapter = ChapterInfo[currentChapterIndex].Data.ChapterID;
            string ChapterTitle = ChapterInfo[currentChapterIndex].Data.ChapterName;

            ChapterManager.Instance.SelectCurrentChapter(currentChapter);

            //Fill up level selection info
            levelSelectionScreen.PopulateLevelsForChapter(currentChapter, ChapterTitle);

            //Go to level selection screen
            UIManager.Instance.ShowLevelSelectionUI();
        }

        public void ResetChapterScroll(bool instant = false)
        {
            currentChapterIndex = 0;

            float targetX = 0f;

            if (instant)
            {
                // Instantly move to start
                chapterContainer.anchoredPosition = new Vector2(targetX, chapterContainer.anchoredPosition.y);
            }
            else
            {
                // Animate to start
                chapterContainer.DOAnchorPosX(targetX, animationDuration).SetEase(Ease.InOutCubic);
            }
        }

    }
}
