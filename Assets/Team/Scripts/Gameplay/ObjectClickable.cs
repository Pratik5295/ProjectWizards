using System.Collections;
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
    private GenericGhosting _ghosting;

    public bool isHovered { get; private set; } = false;
    private bool isClicked = false;

    void Start()
    {
        _ghosting = GetComponentInChildren<GenericGhosting>();

        if (_ghosting)
        {
            onHovered.AddListener(_ghosting.SetGhosting);
        }

        baseCh = GetComponent<Base_Ch>();
        if (baseCh)
        {
            onHovered.AddListener(baseCh.SetGhosting);
            OnEnableClick.AddListener(baseCh.ClickedOnJump);
            OnDisableClick.AddListener(baseCh.ClickedOnJump);
        }

        baseObstacle = GetComponent<Base_Obstacle>();
    }

    private void OnDestroy()
    {
        onHovered.RemoveAllListeners();
        OnEnableClick?.RemoveAllListeners();
        OnDisableClick?.RemoveAllListeners();
    }

    public void HoveredObject()
    {
        if (isHovered || !GameInputManager.Instance.canInteract) return;

        isHovered = true;
        onHovered?.Invoke(true);
    }

    public void UnhoveredObject()
    {
        if (!isHovered || !GameInputManager.Instance.canInteract) return;

        isHovered = false;
        onHovered?.Invoke(false);
    }

    public void ClickedObject()
    {
        if (isClicked) return;

        isClicked = true;
        OnEnableClick?.Invoke();

        StartCoroutine(ResetClicked());
    }

    private IEnumerator ResetClicked()
    {
        yield return new WaitForEndOfFrame();

        isClicked = false;
    }

    public bool ToggleValidity()
    {
        return _ghosting != null && _ghosting.ghostingIsActive;
    }

    public void ShowInfoPanel()
    {
        if (baseCh != null && baseCh.CharacterData != null)
        {
            UIManager.Instance.UpdateInfoPanel(baseCh.CharacterData.Data);
        }
    }
}
