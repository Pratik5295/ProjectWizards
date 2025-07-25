using UnityEngine;

[DefaultExecutionOrder(20)]
public class ObjectClickerManager : MonoBehaviour
{
    public static ObjectClickerManager Instance;

    private GameObject previouslyHovered;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(GameInputManager.Instance.PointerPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Add layer mask for optimization if needed
        {
            ObjectClickable objClickable = hit.collider.GetComponent<ObjectClickable>();

            if (objClickable != null)
            {
                // Handle Hover
                if (objClickable.gameObject != previouslyHovered)
                {
                    HandleUnhoverPrevious();
                    HandleHoverNew(objClickable);
                }

                // Handle Left Click
                if (GameInputManager.Instance.IsClick)
                {
                    objClickable.ClickedObject();
                }

                // Handle Right Click
                if (GameInputManager.Instance.IsRightClick)
                {
                    objClickable.ShowInfoPanel();
                }
            }
            else
            {
                // Unhover if needed
                if (previouslyHovered != null)
                {
                    HandleUnhoverPrevious();
                }
            }
        }
        else
        {
            // No object hit – clear previous hover
            if (previouslyHovered != null)
            {
                HandleUnhoverPrevious();
            }
        }
    }

    private void HandleHoverNew(ObjectClickable newHover)
    {
        previouslyHovered = newHover.gameObject;
        newHover.HoveredObject();
    }

    private void HandleUnhoverPrevious()
    {
        if (previouslyHovered == null) return;

        ObjectClickable prevObj = previouslyHovered.GetComponent<ObjectClickable>();
        if (prevObj != null)
        {
            prevObj.UnhoveredObject();
        }

        previouslyHovered = null;
    }
}
