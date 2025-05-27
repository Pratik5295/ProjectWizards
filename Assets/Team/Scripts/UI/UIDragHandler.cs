using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private LayoutElement layoutElement;
    private Vector2 originalPosition;
    private int originalIndex;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        layoutElement = GetComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        originalPosition = rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;
        layoutElement.ignoreLayout = true;
        transform.SetAsLastSibling(); // ensure it's drawn on top
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Mouse.current.position.ReadValue();
        layoutElement.ignoreLayout = true;
        // Check for reorder
        for (int i = 0; i < originalParent.childCount; i++)
        {
            if (originalParent.GetChild(i) == transform) continue;

            RectTransform other = originalParent.GetChild(i) as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(other, Mouse.current.position.ReadValue()))
            {
                transform.SetSiblingIndex(i);
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        layoutElement.ignoreLayout = false;

        // Snap into position
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
        StartCoroutine(SmoothSnap());
    }

    private IEnumerator SmoothSnap()
    {
        Vector2 targetPos = Vector2.zero;
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPos) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.deltaTime * 10f);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }
}
