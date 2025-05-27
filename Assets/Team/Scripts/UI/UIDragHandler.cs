using UnityEngine;
using UnityEngine.EventSystems;

namespace Team.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected RectTransform rectTransform;
        protected Canvas parentCanvas;
        protected Vector2 offset;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();

            if (parentCanvas == null)
            {
                Debug.LogError("UIDragHandler requires a Canvas parent.");
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            rectTransform.SetAsLastSibling(); // Bring to front
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (parentCanvas == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                parentCanvas.worldCamera,
                out Vector2 localPoint);

            rectTransform.localPosition = localPoint;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            // Hook for subclasses
        }
    }
}
