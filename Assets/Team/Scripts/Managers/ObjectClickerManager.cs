using Team.GameConstants;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(20)]
public class ObjectClickerManager : MonoBehaviour
{
    public ObjectClickerManager Instance;

    private GameObject PreviouslyHovered;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(GameInputManager.Instance.PointerPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //Add a layer mask to this to stop un needed processing of useless objects.
        {
            if (hit.collider.GetComponent<ObjectClickable>())
            {
                ObjectClickable objClickable = hit.collider.gameObject.GetComponent<ObjectClickable>();
                if (objClickable.ToggleValidity() && !objClickable.isHovered && !objClickable.isToggled) { return; }

                if (!objClickable.isHovered && !objClickable.isToggled)
                {
                    HoverClickable(true, hit);
                    //Hover Object.
                }
                if (GameInputManager.Instance.IsClick)
                {
                    LockGhostingVisual(hit, objClickable);
                }
            }
            else
            {
                if (PreviousSelectionCanUnhover())
                {
                    // Should UnHover.
                    HoverClickable(false, hit);
                }
            }
        }
    }

    private bool PreviousSelectionCanUnhover()
    {
        if (!PreviouslyHovered) { return false; }
        ObjectClickable PreviousObjClickable = PreviouslyHovered.GetComponent<ObjectClickable>();

        return PreviouslyHovered && PreviousObjClickable.isHovered && !PreviousObjClickable.isToggled;
    }

    private void HoverClickable(bool shouldHover, RaycastHit hit)
    {
        switch (shouldHover)
        {
            case true:
                    PreviouslyHovered = hit.collider.gameObject;
                    hit.collider.gameObject.GetComponent<ObjectClickable>().HoveredObject();
                break;

            case false:
                    PreviouslyHovered.GetComponent<ObjectClickable>().UnhoveredObject();
                    PreviouslyHovered = null;
                break;                    
        }
    }

    public void LockGhostingVisual(RaycastHit hit, ObjectClickable objClickable)
    {
        if (objClickable.isHovered) 
        {
            objClickable.isHovered = false;
            objClickable.ClickedObject();
        } // This repeating line exists to ensure this doesnt toggle the effect off.

        objClickable.ClickedObject();
    }
}
