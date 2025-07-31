using Team.Managers;
using UnityEngine;

namespace Team.UI
{

    public class UILevelSelectionScreen : UIScreen
    {
        public void OnBackButtonClicked()
        {
            UIManager.Instance.ShowChapterSelectionUI();
        }
    }
}
