using Team.Managers;
using UnityEngine;

namespace Team.UI
{
    public class UITitleScreen : UIScreen
    {
        public void OnNewGameClicked()
        {
            UIManager.Instance.ShowChapterSelectionUI();
        }

        public void OnLoadGameClicked()
        {
            UIManager.Instance.ShowChapterSelectionUI();
        }

        public void OnQuitGameClicked()
        {
            Application.Quit();
        }
    }
}
