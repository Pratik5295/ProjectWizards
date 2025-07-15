using Team.Data;
using Team.Managers;
using UnityEngine;

namespace Team.UI
{
    public class UIGameScreen : UIScreen
    {
        [SerializeField]
        private GameObject tutorialUI;

        [SerializeField]
        private UIInforPanel inforPanel;

        public void PopulateInfoPanel(CharacterDataStruct _data)
        {
            inforPanel.Populate(_data);
        }

        public void ShowTutorialUI()
        {
            tutorialUI.SetActive(true);

            TutorialManager.Instance.StartTutorial();
        }

        public void HideTutorialUI()
        {
            tutorialUI.SetActive(false);
        }
    }
}
