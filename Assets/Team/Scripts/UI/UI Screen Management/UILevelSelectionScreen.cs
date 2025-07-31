using Team.Gameplay.LevelSystem;
using Team.Managers;
using UnityEngine;

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

        public void PopulateAllLevels()
        {

        }
    }
}
