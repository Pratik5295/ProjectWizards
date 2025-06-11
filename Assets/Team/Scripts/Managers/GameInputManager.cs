using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    private InputSystem_Actions _inputActions;

    public Vector2 PointerPosition => _inputActions.UI.Point.ReadValue<Vector2>();
    public Vector2 PointerDelta => _inputActions.UI.Drag.ReadValue<Vector2>();
    public bool IsPointerPressed => _inputActions.UI.Click.IsPressed();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
            _inputActions.Disable();
    }
}
