using Team.Managers;
using UnityEngine;

namespace Team.UI
{
    public class UITitleScreen : UIScreen
    {
        public void OnNewGameClicked()
        {
            UIManager.Instance.ShowLevelSelectionUI();
        }

        public void OnLoadGameClicked()
        {
            UIManager.Instance.ShowLevelSelectionUI();
        }

        public void OnQuitGameClicked()
        {
            Application.Quit();
        }
    }
}
