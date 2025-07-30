using Team.Managers;
using UnityEngine;
using System.Collections.Generic;
using Team.Gameplay.LevelSystem;
using UnityEngine.Android;

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
            var data = ChapterDataList[currentChapterIndex];
            UIChapterInfo info = new UIChapterInfo(data.ChapterName, data.ChapterSprite, data.ChapterNumberSprite);
            UIChapter.PopulateChapterInfo(info);
        }

        public void GoBackToMainMenu()
        {
            UIManager.Instance.ShowMenuUI();
        }

        public void OnSelectButtonClicked()
        {
            //Go to level selection screen
        }
    }
}
