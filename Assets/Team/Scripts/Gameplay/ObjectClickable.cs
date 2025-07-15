using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ObjectClickable : MonoBehaviour
{

    public UnityEvent onHovered;
    public UnityEvent OnEnableClick;
    public UnityEvent OnDisableClick;

    private Base_Ch baseCh;
    private Base_Obstacle baseObstacle;
    
    GenericGhosting _ghosting;
    public bool isHovered = false;

    void Start()
    {
        if (GetComponentInChildren<GenericGhosting>())
        {
            _ghosting = GetComponentInChildren<GenericGhosting>();
            
            onHovered.AddListener(_ghosting.toggleGhosting);
            OnEnableClick.AddListener(_ghosting.enableGhosting);
            OnDisableClick.AddListener(_ghosting.disableGhosting);

            if (GetComponent<Base_Ch>())
            {
                baseCh = GetComponent<Base_Ch>();
                onHovered.AddListener(baseCh.ShowHideOutline);
                OnEnableClick.AddListener(baseCh.ClickedOnJump);
                OnDisableClick.AddListener(baseCh.ClickedOnJump);
            }

            if (GetComponent<Base_Obstacle>())
            {
                baseObstacle = GetComponent<Base_Obstacle>();
            }
        }
        Debug.Log($"has initiailised character");
    }

    public void HoveredObject()
    {
        if (_ghosting == null) { return; }
        if (_ghosting.ghostingIsActive)
        {
            return;
        }
        isHovered = true;
        _ghosting.isHovered = true;
        onHovered.Invoke();
    }
    public void UnhoveredObject()
    {
        if (_ghosting == null) { return; }
        if (_ghosting.ghostingIsActive && !isHovered){ return;}
        isHovered = false;
        _ghosting.isHovered = false;
        onHovered.Invoke();
    }

    public void ClickedObject()
    {
        if (_ghosting == null) { return; }
        if (_ghosting.isHovered)
        {
            _ghosting.ghostingIsActive = false;
            OnEnableClick.Invoke();
            return;
        }
        OnDisableClick.Invoke();
    }

    public bool ToggleValidity()
    {
        if (_ghosting)
        {
            return _ghosting.ghostingIsActive;
        }
        return false;
    }
}
