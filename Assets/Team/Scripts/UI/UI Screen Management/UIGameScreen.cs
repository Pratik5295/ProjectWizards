using DG.Tweening;
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

        [SerializeField]
        private CanvasGroup turnOrderParent;

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


        public void ShowTurnOrderParent()
        {
            //turnOrderParent.DOFade(1, 0.2f).OnStart(() =>
            //{
            //    turnOrderParent.interactable = true;
            //    turnOrderParent.blocksRaycasts = true;
            //});
        }

        public void HideTurnOrderParent()
        {
            //turnOrderParent.DOFade(0, 0.2f).OnComplete(() =>
            //{
            //    turnOrderParent.interactable = false;
            //    turnOrderParent.blocksRaycasts = false;
            //});
        }
    }
}
