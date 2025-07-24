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
                if (!objClickable) { return; }

                if (GameInputManager.Instance.IsClick)
                {
                    LockGhostingVisual(hit, objClickable);
                }

                if (!objClickable.isHovered)
                {
                    HoverClickable(true, hit);
                    //Hover Object.
                }
               
                if (GameInputManager.Instance.IsRightClick)
                {
                    ShowInfoPanel(objClickable);
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
        if (!PreviousObjClickable)
        {
            return false;
        }

        return PreviouslyHovered && PreviousObjClickable.isHovered;
    }

    private void HoverClickable(bool shouldHover, RaycastHit hit)
    {
        if(hit.collider.gameObject.GetComponent<ObjectClickable>() != PreviouslyHovered) 
        {
            if (PreviousSelectionCanUnhover())
            {
                PreviouslyHovered.GetComponent<ObjectClickable>().UnhoveredObject();
            }
        }

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
        objClickable.ClickedObject();
    }

    private void ShowInfoPanel(ObjectClickable objClickable)
    {
        objClickable.ShowInfoPanel();
    }
}
