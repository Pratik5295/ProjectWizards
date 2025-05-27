using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Team.UI;

namespace Team.UI.Gameplay
{
    public class UIGameTurn : UIDragHandler
    {
        private LayoutElement layoutElement;
        private Transform originalParent;
        private int originalIndex;

        protected override void Awake()
        {
            base.Awake();
            layoutElement = GetComponent<LayoutElement>();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);


            originalParent = transform.parent;
            originalIndex = transform.GetSiblingIndex();

            layoutElement.ignoreLayout = true;
            rectTransform.SetAsLastSibling();  // Bring to front

            // Optional: disable raycast so we don’t block UI under dragged item
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (cg == null)
                cg = gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            // Move freely based on pointer position (local to canvas)
            if (parentCanvas == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                parentCanvas.worldCamera,
                out Vector2 localPoint);

            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, localPoint.y, rectTransform.localPosition.z);

            // Now reorder siblings dynamically based on Y position
            int newIndex = originalParent.childCount - 1;
            for (int i = 0; i < originalParent.childCount; i++)
            {
                Transform sibling = originalParent.GetChild(i);
                if (sibling == transform) continue;

                if (rectTransform.position.y > sibling.position.y)
                {
                    newIndex = i;
                    break;
                }
            }
            transform.SetSiblingIndex(newIndex);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            layoutElement.ignoreLayout = false;

            // Snap back to layout position
            rectTransform.anchoredPosition = Vector2.zero;

            // Re-enable raycast blocking
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (cg != null)
                cg.blocksRaycasts = true;

            // Force layout update
            LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent as RectTransform);
            Canvas.ForceUpdateCanvases();
        }
    }
}
