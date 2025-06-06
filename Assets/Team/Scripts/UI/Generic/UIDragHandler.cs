using System;
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
    private int newIndex; //Final index set after the drag has been completed

    [SerializeField]
    private float offsetY;

    [SerializeField]
    private float posY; //Constant y position

    public Action<int> OnSiblingIndexUpdatedEvent;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        layoutElement = GetComponent<LayoutElement>();
    }

    protected virtual void Start()
    {
        StartCoroutine(GetAccuratePosition());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        originalPosition = rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;
        layoutElement.ignoreLayout = true;
        transform.SetAsLastSibling(); // Ensure it's drawn on top
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 newMovePos = new Vector2(mousePos.x, posY);
        rectTransform.position = newMovePos;
        layoutElement.ignoreLayout = true;

        float draggedX = newMovePos.x;
        int newIndex = -1;

        for (int i = 0; i < originalParent.childCount; i++)
        {
            if (originalParent.GetChild(i) == transform) continue;

            RectTransform other = originalParent.GetChild(i) as RectTransform;
            float otherX = other.position.x;

            //TODO: Math check make it better by using offset and considering spacing etc
            // If mouse is above this child, insert before it
            if (draggedX > otherX)
            {
                newIndex = i;
                break;
            }
        }

        // If we didn’t find any valid spot, insert at the end
        if (newIndex == -1)
        {
            newIndex = originalParent.childCount - 1;
        }

        transform.SetSiblingIndex(newIndex);
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

        newIndex = transform.GetSiblingIndex();

        if(originalIndex != newIndex)
        {
            originalIndex = newIndex;
            OnSiblingIndexUpdatedEvent?.Invoke(newIndex);
        }
    }

    private IEnumerator GetAccuratePosition()
    {
        yield return new WaitForEndOfFrame(); // Wait until layout system finishes

        Vector3 accurateWorldPos = rectTransform.position;
        posY = accurateWorldPos.y + offsetY;
    }
}
