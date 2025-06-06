using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Team.MetaConstants
{
    public static partial class MetaConstants
    {
        public const float UISnapThreshold = 0.1f;
        public const float UICardMoveSpeed = 10f;
    }
}

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private LayoutElement layoutElement;
    private int originalIndex;
    private int newIndex;

    [SerializeField]
    private float offsetX;

    public Action<int> OnSiblingIndexUpdatedEvent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        layoutElement = GetComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;
        layoutElement.ignoreLayout = true;
        transform.SetAsLastSibling();

        // Calculate offset for smooth dragging
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            originalParent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPos
        );
        offsetX = rectTransform.anchoredPosition.x - localPointerPos.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameInputManager.Instance.IsPointerPressed) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            originalParent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        rectTransform.anchoredPosition = new Vector2(localPoint.x + offsetX, rectTransform.anchoredPosition.y);

        newIndex = -1;

        for (int i = 0; i < originalParent.childCount; i++)
        {
            if (originalParent.GetChild(i) == transform) continue;

            RectTransform sibling = originalParent.GetChild(i) as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(sibling, eventData.position, eventData.pressEventCamera))
            {
                newIndex = i;
                transform.SetSiblingIndex(i);
                break;
            }
        }

        if (newIndex == -1)
        {
            newIndex = originalParent.childCount - 1;
            transform.SetSiblingIndex(newIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        layoutElement.ignoreLayout = false;

        transform.SetParent(originalParent);
        StartCoroutine(FinalizeDrag());
    }

    private IEnumerator FinalizeDrag()
    {
        // Wait one frame to allow Unity to settle layout system
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent as RectTransform);

        if (originalIndex != newIndex)
        {
            originalIndex = newIndex;
            OnSiblingIndexUpdatedEvent?.Invoke(newIndex);
        }
    }
}
