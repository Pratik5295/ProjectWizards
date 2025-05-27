using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Transform originalParent;
    private int originalSiblingIndex;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;  // Make sure other elements receive events during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move element with the cursor
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // Snap back if no valid drop
        if (transform.parent == originalParent)
            transform.SetSiblingIndex(originalSiblingIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged != null && dragged != gameObject)
        {
            // Insert dragged element before this element
            dragged.transform.SetParent(transform.parent);
            int thisIndex = transform.GetSiblingIndex();
            dragged.transform.SetSiblingIndex(thisIndex);
        }
    }
}
