using Team.Gameplay.LevelSystem;
using Team.Managers;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using System.Linq;
using TMPro;

namespace Team.UI
{

    public class UILevelSelectionScreen : UIScreen
    {
        [SerializeField]
        private UILevel[] levelBoxes = new UILevel[6];

        [SerializeField]
        private UILevelPanel infoPanel;

        [SerializeField]
        private TextMeshProUGUI ChapterTitle;


        public void OnBackButtonClicked()
        {
            UIManager.Instance.ShowChapterSelectionUI();
        }

        public void PopulateLevelsForChapter(ChapterID _currentChapter, string _chapterTitle)
        {
            ChapterTitle.text = _chapterTitle;
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

        public void OnSetLevelInfoPanel(LevelData _info)
        {
            string title = _info.Stats.LevelName;
            string description = _info.Stats.LevelDescription;
            infoPanel.PopulateData(title,description,_info.Stats.LevelObjectives);
        }

        public void PlaySelectedLevel()
        {
            LevelManager.Instance.LoadCurrentLevel();
        }
    }
}
