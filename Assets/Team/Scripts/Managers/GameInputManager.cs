using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    private InputSystem_Actions _inputActions;

    public Vector2 PointerPosition => _inputActions.UI.Point.ReadValue<Vector2>();
    public Vector2 PointerDelta => _inputActions.UI.Drag.ReadValue<Vector2>();
    public bool IsPointerPressed => _inputActions.UI.Click.IsPressed();

    [Header("Click/Drag Settings")]
    [SerializeField] private float dragThreshold = 10f; // pixels

    private Vector2 _pointerDownPosition;
    private float _pointerDownTime;
    private bool _isDragging;
    private bool _pointerPreviouslyPressed;

    public bool IsDragging => _isDragging;
    public bool IsClick { get; private set; }

    public bool IsRightClick;

    public bool canInteract = true;

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

    private void Update()
    {
        if (!canInteract) return;
        HandleClickAndDrag();

        IsRightClick = _inputActions.UI.RightClick.IsPressed();
    }

    private void LateUpdate()
    {
        IsClick = false;
    }

    private void HandleClickAndDrag()
    {
        bool isPressed = IsPointerPressed;

        if (isPressed && !_pointerPreviouslyPressed)
        {
            // Pointer just pressed
            _pointerDownPosition = PointerPosition;
            _pointerDownTime = Time.time;
            _isDragging = false;
            IsClick = false;
        }

        if (isPressed)
        {
            float dragDistance = Vector2.Distance(PointerPosition, _pointerDownPosition);

            if (!_isDragging && dragDistance > dragThreshold)
            {
                //Drag has been detected, turn ghosting off
                _isDragging = true;
            }
        }
        else if (_pointerPreviouslyPressed)
        {
            // Pointer just released
            if (!_isDragging)
            {
                //Only click is detected
                IsClick = true;
            }

            _isDragging = false;
        }

        _pointerPreviouslyPressed = isPressed;
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
            _inputActions.Disable();
    }
}
