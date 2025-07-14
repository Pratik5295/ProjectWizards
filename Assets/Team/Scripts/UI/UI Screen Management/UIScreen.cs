using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.UI
{

    public class UIScreen : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        protected CanvasGroup canvasGroup;

        public GameScreen screenType;

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
