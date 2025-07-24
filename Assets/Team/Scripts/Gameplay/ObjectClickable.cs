using Team.Managers;
using UnityEngine;
using UnityEngine.Events;

public class ObjectClickable : MonoBehaviour
{

    public UnityEvent<bool> onHovered;
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
            
            onHovered.AddListener(_ghosting.SetGhosting);
            //OnEnableClick.AddListener(_ghosting.enableGhosting);
            //OnDisableClick.AddListener(_ghosting.disableGhosting);

            if (GetComponent<Base_Ch>())
            {
                baseCh = GetComponent<Base_Ch>();
                onHovered.AddListener(baseCh.SetGhosting);
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

    private void OnDestroy()
    {
        onHovered.RemoveAllListeners();
        OnEnableClick?.RemoveAllListeners();
        OnDisableClick?.RemoveAllListeners();
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
        onHovered?.Invoke(true);
    }
    public void UnhoveredObject()
    {
        if (_ghosting == null) { return; }
        if (_ghosting.ghostingIsActive && !_ghosting.isHovered)
        {
            isHovered = false; 
            return;
        }
        isHovered = false;
        _ghosting.isHovered = false;
        onHovered?.Invoke(false);
    }

    public void ClickedObject()
    {
        if (_ghosting == null) { return; }
        if (_ghosting.isHovered)
        {
            _ghosting.isHovered = false;
            OnEnableClick.Invoke();
            //_ghosting.ghostingIsActive = false;
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

    public void ShowInfoPanel()
    {
        UIManager.Instance.UpdateInfoPanel(baseCh.CharacterData.Data);
    }
}
