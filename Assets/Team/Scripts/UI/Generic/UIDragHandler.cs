using System;
using System.Collections;
using Team.Gameplay.TurnSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float UISnapThreshold = 0.1f;
        public const float UICardMoveSpeed = 10f;
    }
}

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private TurnHolder _turnHolder;

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
        originalParent = transform.parent;

        _turnHolder = originalParent.GetComponent<TurnHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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

        transform.SetAsLastSibling();
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

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        layoutElement.ignoreLayout = false;

        newIndex = _turnHolder.GetIndex(rectTransform.localPosition.x);
        transform.SetSiblingIndex(newIndex);

        transform.SetParent(originalParent);
        StartCoroutine(FinalizeDrag());
    }

    private IEnumerator FinalizeDrag()
    {
        // Wait one frame to allow Unity to settle layout system
        yield return new WaitForEndOfFrame();

        CanvasUpdater();
    }

    private void CanvasUpdater()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent as RectTransform);


        if (originalIndex != newIndex)
        {
            originalIndex = newIndex;
            OnSiblingIndexUpdatedEvent?.Invoke(newIndex);
        }
    }
}
