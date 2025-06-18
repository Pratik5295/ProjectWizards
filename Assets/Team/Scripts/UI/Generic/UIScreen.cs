using UnityEngine;

namespace Team.UI
{

    public class UIScreen : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private CanvasGroup canvasGroup;

        public virtual void Start()
        {
            if(canvasGroup == null)
            {
                Debug.LogError("Missing Canvas Group on Screen",gameObject);
            }
        }

        public virtual void OnShow()
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public virtual void OnHide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
