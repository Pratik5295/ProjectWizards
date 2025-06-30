using UnityEngine;
using UnityEngine.UI;

namespace Team.Gameplay.TurnSystem
{
    public class GameBreakpoint : MonoBehaviour
    {
        [SerializeField]
        private Image sliderImage;

        [SerializeField]
        private CanvasGroup canvasGroup;

        private void Start()
        {
            sliderImage = GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void MakeInteractable()
        {
            sliderImage.raycastTarget = true;
            canvasGroup.interactable = true;
        }

        public void MakeUnInteractable()
        {
            sliderImage.raycastTarget = false;
            canvasGroup.interactable = false;
        }
    }
}
