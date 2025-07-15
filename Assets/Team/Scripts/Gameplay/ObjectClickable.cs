using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ObjectClickable : MonoBehaviour
{

    public UnityEvent clickedOn;

    Base_Obstacle obstacle;
    Base_Ch base_Ch;

    public bool isToggled = false;
    public bool isHovered = false;

    void Start()
    {
        if (GetComponent<Base_Ch>())
        {
            base_Ch = GetComponent<Base_Ch>();
            clickedOn.AddListener(base_Ch.ToggleGhosting);
        }
        else if (GetComponent<Base_Obstacle>())
        {
            obstacle = GetComponent<Base_Obstacle>();
            clickedOn.AddListener(obstacle.ToggleVisualisation);
        }
        Debug.Log($"has initiailised character");
    }

    public void HoveredObject()
    {
        isHovered = true;
        clickedOn.Invoke();
    }
    public void UnhoveredObject()
    {
        isHovered = false;
        clickedOn.Invoke();
    }

    public void ClickedObject()
    {
        clickedOn.Invoke();
    }

    public bool ToggleValidity()
    {
        if (base_Ch)
        {
            return base_Ch._ghosting.ghostingIsActive;
        }
        else if (obstacle)
        {
            return obstacle._ghosting.ghostingIsActive;
        }
        return false;
    }
}
