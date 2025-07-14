using Team.Managers;
using UnityEngine;

namespace Team.UI
{
    public class UIGameScreen : UIScreen
    {
        [SerializeField]
        private GameObject tutorialUI;

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
