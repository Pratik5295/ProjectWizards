using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ObjectClickable : MonoBehaviour
{

    public UnityEvent clickedOn;

    Base_Obstacle obstacle;
    Base_Ch base_Ch;

    bool isToggled = false;
    bool isHovered = false;
    public bool IsToggled
    {
        get { return isToggled; }
    }

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
        }
        Debug.Log($"has initiailised character");
    }

    public void ClickedObject()
    {
        isToggled = !isToggled;
        clickedOn.Invoke();
    }
}
