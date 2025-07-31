using Team.Gameplay.LevelSystem;
using Team.Managers;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using System.Linq;

namespace Team.UI
{

    public class UILevelSelectionScreen : UIScreen
    {
        [SerializeField]
        private UILevel[] levelBoxes = new UILevel[6];


        public void OnBackButtonClicked()
        {
            UIManager.Instance.ShowChapterSelectionUI();
        }

        public void PopulateLevelsForChapter(ChapterID _currentChapter)
        {
            //Get List 

            var list 
                = LevelManager.Instance.AllLevels.Where(x => x.chapterID == _currentChapter).ToList();

            int index = 0;

            foreach(var level in list)
            {
                levelBoxes[index].PopulateLevelInfo(level.LevelData);

                index++;
            }
        }

        public UILevel GetLevelBox(LevelID _id)
        {
            foreach(var level in levelBoxes)
            {
                if(level.LevelID == _id)
                {
                    return level;
                }
            }

            return null;
        }
    }
}
