using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameInputManager : MonoBehaviour
{
    public PlayerInput input;

    public Vector2 startPosition;
    public Vector2 currentPosition;
    public Vector2 prevPosition;


    public bool isClicked = false;

    // Reference to GraphicRaycaster on your Canvas
    public GraphicRaycaster graphicRaycaster;

    // Reference to EventSystem in scene
    public EventSystem eventSystem;

    public Action<Vector2> OnMouseDownEvent;
    public Action<Vector2> OnMouseUpEvent;
    public Action<float> OnMouseDragEvent;

    [SerializeField]
    private IInputDetectable uiElement = null;

    public void OnPoint(InputValue inputValue)
    {
        currentPosition = inputValue.Get<Vector2>();
        //Debug.Log($"Pointing at: {currentPosition}");
        if (isClicked)
        {
            if (CheckRaycast())
            {
                uiElement.OnElementDragged();
            }
        }
    }

    public void OnClick(InputValue inputValue)
    {
        isClicked = inputValue.isPressed;

        if(uiElement != null)
        {
            if (isClicked)
            {
                //Pressed down on it
                uiElement.OnElementDown();

            }
            else
            {
                //Released
                uiElement.OnElementUp();
                uiElement = null;
            }
        }

    }

    private bool CheckRaycast()
    {
        if (graphicRaycaster == null || eventSystem == null)
        {
            Debug.LogWarning("GraphicRaycaster or EventSystem reference missing!");
            return false;
        }

        // Set up PointerEventData at the current pointer position
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = currentPosition;

        // Raycast using the GraphicRaycaster and collect results
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                Debug.Log($"UI Element hit: {result.gameObject.name}");

                if (result.gameObject.TryGetComponent<IInputDetectable>(out uiElement))
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.Log("No UI element hit by raycast.");
            uiElement = null;
        }

        return false;
    }
}
