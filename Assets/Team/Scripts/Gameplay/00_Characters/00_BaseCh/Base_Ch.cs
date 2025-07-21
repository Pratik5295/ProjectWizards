using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Team.Data;
using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.Characters;
using Team.Gameplay.GridSystem;
using Team.Managers;
using Team.UI;
using UnityEngine;
using static Team.GameConstants.MetaConstants;

[System.Serializable]
public class PlayerMove
{
    //FUTURE SCARE: Update the previous Rotation here. So the character can rotate back to previous move's rotation
    public TileID movedFrom;
    public bool wasMoved;

    public PlayerMove(TileID _movedFrom, bool _move)
    {
        movedFrom = _movedFrom;
        wasMoved = _move;
    }
}

[DefaultExecutionOrder(2)]
public class Base_Ch : MonoBehaviour, IMoveable, IProjectileHittable, IUsableAbility, IDestroyable
{
    [SerializeField]
    private CharacterDataSO characterData;  //The SO file that will be used to update Info Panel.
    public CharacterDataSO CharacterData { get; private set; }


    [Header("Enumerations")]
    [SerializeField] private Enum_CharacterState CharState = Enum_CharacterState.Alive;

    public bool IsAlive => CharState == Enum_CharacterState.Alive;

    public Stack<PlayerMove> HistoryStack = new Stack<PlayerMove>();


    [Header("Script References")]
    [SerializeField] protected GridManager ref_gridManager;

    [SerializeField] protected Base_Rotation baseRotation;


    //Turn Completion Handler
    protected TaskCompletionSource<bool> undoAwaiter = new TaskCompletionSource<bool>();
    public Base_Rotation BaseRotation
    {
        get { return baseRotation; }
    }

    public CharacterReskinner CharacterReskinner;


    #region Tile Variables
    [Space(5)]
    [Header("Movement Variables")]
    [Header("---Tile Variables---")]
    [SerializeField] protected TileID _currentTileID = new TileID(0, 0);
    [SerializeField] protected TileID _previousTileID = new TileID(0, 0);
    [SerializeField] protected TileID _startTileID = new TileID(0, 0);
    private Enum_GridDirection startingDirection;
    public TileID CurrentTileID
    {
        get { return _currentTileID; }
    }
    private GridTile _currentTile;
    private GridTile _previousTile;


    private float OffsetValue;
    private float smoothingTime = 1f; //Time to reach the target position.
    private float currentTime; //Current elapsed Time for movement lerp.
    private float lerpingDelayTime = 0.001f;

    [Space(5)]
    [Header("Y Offset and Movement Jump Variables")]
    [SerializeField]
    private float ySpawnOffset = 1.5f;

    public float YSpawnOffset => ySpawnOffset;

    [SerializeField]
    private float ydefaultOffset = 1.5f;

    [SerializeField] private AnimationCurve _yMovementCurve;

    private int movementIteration;

    private Vector3 startPosition;

    private bool alreadyMoving;

    public PushProjectile PushProjectileInstance;
    #endregion

    #region Vars_InvalidMovementShake

    [Space(5)]
    [Header("Invalid Move Shake")]
    private float shakeTimer = 0f;

    private float maxShakeAmount = 0.3f;
    #endregion

    #region Mesh And Collider

    [Space(5)]
    [Header("Colliders and Mesh")]
    private Collider _collider;
    private MeshRenderer _meshRenderer;
    #endregion

    #region Ghosting Section

    [Header("Ghosting Section")]

    public ObjectClickable GhostManager
    {
        get; private set;
    }

    [SerializeField]
    public GenericGhosting _ghosting;

    public bool IsGhosting = false;

    bool isJumping = false;

    #endregion

    #region Character Barking Section

    [Space(5)]
    [Header("Barking Section")]

    [SerializeField]
    protected CharacterBark _characterBark;
    protected UICharacter _characterUI => _characterBark.GetComponent<UICharacter>();

    #endregion

    public System.Action OnStateChanged;

    public System.Action OnTurnComplete;

    [ContextMenu("Initialise this Character")]
    public virtual void InitialiseCharacter(TileID StartingTileID, Enum_GridDirection _startingDirection)
    {
        CharacterData = characterData;

        ref_gridManager = GridManager.Instance;
        OffsetValue = MetaConstants.GridSlot_Offset;

        _currentTileID = StartingTileID;
        _previousTileID = _currentTileID;
        _startTileID = _currentTileID;

        _currentTile = ref_gridManager.FindTile(_currentTileID);
        _currentTile.SetObjectOccupyingTile(this.gameObject);

        _previousTile = _currentTile;

        baseRotation = GetComponent<Base_Rotation>();
        startingDirection = _startingDirection;
        ResetRotationToStart();

        GhostManager = GetComponent<ObjectClickable>();

        CharacterReskinner = GetComponent<CharacterReskinner>();

        transform.position = new Vector3(_currentTile.TilePosition.x, _currentTile.TilePosition.y + ySpawnOffset, _currentTile.TilePosition.z);

        _collider = GetComponent<Collider>();
        _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    void Start()
    {
        ref_gridManager = GridManager.Instance;
    }

    #region Debugging Movement Button Functions
    public void buttonForwardBackward(bool moveForward)
    {
        if (alreadyMoving)
        {
            return;
        }
        if (moveForward)
        {
            StartCoroutine(MoveByAmount(2, new Vector2(0, 1)));
        }
        else
        {
            StartCoroutine(MoveByAmount(2, new Vector2(0, -1)));
        }
    }
    public void buttonLeftRight(bool moveRight)
    {
        if (alreadyMoving)
        {
            return;
        }
        if (moveRight)
        {
            StartCoroutine(MoveByAmount(2, new Vector2(1, 0)));
        }
        else
        {
            StartCoroutine(MoveByAmount(2, new Vector2(-1, 0)));
        }
    }
    #endregion

    //Moves by a defined amount in a direction, if the tile exists and player can move there. Then passes to lerp.
    public virtual IEnumerator MoveByAmount(int movementAmount, Vector2 dir, bool wasPushed = false)
    {
        _previousTileID = _currentTileID;
        _previousTile = ref_gridManager.FindTile(_previousTileID);
        _previousTile.UpdateOccupiedStatus(false);

        for (int i = 0; i < movementAmount; i++)
        {
            alreadyMoving = true;
            Vector3 desiredLocation = new Vector3(_currentTileID.x + (dir.x * OffsetValue), transform.position.y, _currentTileID.y + (dir.y * OffsetValue));

            TileID desiredTileID = new TileID(_currentTileID.x + (int)dir.x, _currentTileID.y + (int)dir.y);
            GridTile targetTile = ref_gridManager.FindTile(desiredTileID);


            if (targetTile && targetTile.IsTileWalkable())
            {
                if (targetTile.IsIceTile())
                {
                    movementAmount = IceTileLogic(movementAmount);
                    wasPushed = true;
                }
                if (_currentTile.IsIceTile() && !targetTile.IsIceTile())
                {
                    smoothingTime = 1f;
                }

                Vector3 targetPosition = new Vector3(targetTile.TilePosition.x, desiredLocation.y, targetTile.TilePosition.z);

                _currentTileID = targetTile.TileID;
                _currentTile = ref_gridManager.FindTile(_currentTileID);

                yield return StartCoroutine(LerpingMovement(targetPosition, wasPushed));
            }
            else
            {
                StartCoroutine(ShakeCharacter(0.25f));
                alreadyMoving = false;
                if (wasPushed)
                {
                    WasPushed(wasPushed);
                    yield break;
                }

                OnAbilityCompleted();
                yield break;
            }
        }

        _currentTile.UpdateOccupiedStatus(true, gameObject);

        PlayerMove playerMove = new PlayerMove(_previousTileID, true);
        HistoryStack.Push(playerMove);

        if (wasPushed)
        {
            WasPushed(wasPushed);
            yield break;
        }

        OnAbilityCompleted();
    }


    //Lerps the movement to the next available tile.
    public virtual IEnumerator LerpingMovement(Vector3 targetPosition, bool wasPushed = false)
    {
        currentTime = 0;
        //Vector3 startingPosition = transform.position;
        float positionYLerped = ydefaultOffset;
        while (currentTime < smoothingTime)
        {
            currentTime += Time.deltaTime;

            float lerpAmount = currentTime / smoothingTime;

            if (!wasPushed)
            {
                positionYLerped = Mathf.Lerp(transform.position.y, ydefaultOffset + _yMovementCurve.Evaluate(currentTime), lerpAmount);
            }

            transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, lerpAmount), positionYLerped, Mathf.Lerp(transform.position.z, targetPosition.z, lerpAmount));

            yield return null;
        }

        if (currentTime >= smoothingTime)
        {
            currentTime = 0;
            transform.position = targetPosition;
            startPosition = transform.position;

            CheckTileStatus();
        }

        alreadyMoving = false;
    }

    private int IceTileLogic(int movementAmount)
    {
        if (!_currentTile.IsIceTile())
        {
            movementAmount = 0; movementAmount += 2;
        }
        else movementAmount++;

        smoothingTime = .1f;
        return movementAmount;
    }

    [ContextMenu("Undo Movement")]
    public virtual async Task UndoAction()
    {
        Debug.Log($"Pratik starting awaiting on: {gameObject.name}");
        await CharacterUndoStack();

        Debug.Log($"Pratik awaiting completed. Well done {gameObject.name}");

        OnTurnComplete?.Invoke();
    }

    protected void HistoryStackIsEmpty()
    {
        if (HistoryStack.Count == 0)
        {
            if(undoAwaiter != null && !undoAwaiter.Task.IsCompleted)
            {
                undoAwaiter.SetResult(true);
            }
            
        }
    }

    protected virtual async Task CharacterUndoStack()
    {
        undoAwaiter = new TaskCompletionSource<bool>();

        if (HistoryStack.Count == 0)
        {
        }
        else
        {
            while (HistoryStack.Count > 0)
            {
                var move = HistoryStack.Pop();

                if (move.wasMoved)
                {
                   await UndoMovement(move.movedFrom);
                }
            }
            await undoAwaiter.Task;
        }
    
    }

    protected TaskCompletionSource<bool> undoAbilityWaiter = new TaskCompletionSource<bool>();

    public async Task UndoMovement(TileID _moveToTileID)
    {
        undoAbilityWaiter = new TaskCompletionSource<bool>();
        if (_currentTileID == _moveToTileID)
        {
            var tile = ref_gridManager.FindTile(_moveToTileID);
            transform.position = new Vector3(tile.TilePosition.x, transform.position.y, tile.TilePosition.z);
            //return; 
        }
        _currentTile.UpdateOccupiedStatus();

        _currentTileID = _moveToTileID;
        _currentTile = ref_gridManager.FindTile(_currentTileID);

        _currentTile.SetObjectOccupyingTile(this.gameObject);

        transform.position = new Vector3(_currentTile.TilePosition.x, transform.position.y, _currentTile.TilePosition.z);

        ResetRotationToStart();

        await undoAbilityWaiter.Task;
    }

    private void ResetRotationToStart()
    {
        baseRotation.changeFacingDirection(startingDirection);
        Vector2 v2Dir = baseRotation.dirToV2(baseRotation.DirectionFacing);
        baseRotation.RotateToFaceDir(v2Dir);

        OnUndoAbilityCompleted();
    }

    //Shakes character if path or tile is invalid.
    protected IEnumerator ShakeCharacter(float MaxShakeTime)
    {
        Vector3 defaultPos = transform.localPosition;

        shakeTimer = 0f;

        while (shakeTimer < MaxShakeTime)
        {
            transform.localPosition = defaultPos;
            shakeTimer += Time.deltaTime;

            float shakeAmountX = Random.Range(-0.3f, 0.4f) * maxShakeAmount;
            float shakeAmountZ = Random.Range(-0.3f, 0.4f) * maxShakeAmount;

            transform.localPosition = new Vector3(transform.localPosition.x + shakeAmountX, transform.localPosition.y, transform.localPosition.z + shakeAmountZ);

            yield return null;
        }
        transform.localPosition = defaultPos;
    }

    private void WasPushed(bool wasPushed)
    {
        if (wasPushed)
        {
            if (PushProjectileInstance)
            {
                PushProjectileInstance.CleanUp();
                PushProjectileInstance = null;
            }
        }
    }

    public void UpdateCurrentTileID()
    {
        _currentTileID = _currentTile.TileID;
    }

    public void SetCurrentTile(TileID updatedTileID, GridTile updatedGridTile)
    {
        _currentTileID = updatedTileID;
        _currentTile = updatedGridTile;
    }

    private void CheckTileStatus()
    {
        if (_currentTile.IsDeathTile())
        {
            KillCharacter();
            OnAbilityCompleted();
            OnTurnComplete?.Invoke(); //FUTURE SCARE: Maybe will cause an issue
        }
        if (_currentTile.IsIceTile())
        {

        }
    }


    public virtual void HitByProjectile(Enum_ProjectileType projectileType)
    {
        switch (projectileType)
        {
            case Enum_ProjectileType.Fireball:
                KillCharacter();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.s_death, this.transform.position);
                break;

            case Enum_ProjectileType.NonLethalRound:
                CharState = Enum_CharacterState.Incapacitated;
                break;
        }
        OnStateChanged?.Invoke();
    }

    private void KillCharacter()
    {
        CharState = Enum_CharacterState.Dead;

        DisableObject();
    }

    public bool checkIfCharAlive()
    {
        if (CharState == Enum_CharacterState.Alive) { return true; }
        else return false;
    }

    public void resetCharState(bool isResettingTurn = false)
    {
        if (CharState == Enum_CharacterState.Incapacitated)
        {
            OnStateChanged?.Invoke();
            CharState = Enum_CharacterState.Alive;
        }

        if (CharState == Enum_CharacterState.Dead && isResettingTurn)
        {
            OnStateChanged?.Invoke();
            CharState = Enum_CharacterState.Alive;
        }
    }

    protected TaskCompletionSource<bool> abilityWaiter = new TaskCompletionSource<bool>();

    public async virtual Task UseAbility()
    {
        abilityWaiter = new TaskCompletionSource<bool>();
        // Debug.LogError($" {gameObject.name} Ability not programmed for character");
        StartCoroutine(MoveByAmount(1, baseRotation.GetFacingDirection()));

        await abilityWaiter.Task;

    }

    protected void OnAbilityCompleted()
    {
        if (abilityWaiter != null && !abilityWaiter.Task.IsCompleted)
        {
            abilityWaiter.SetResult(true);
        }

        OnTurnComplete?.Invoke();
    }

    protected void OnUndoAbilityCompleted()
    {
        if(undoAbilityWaiter != null && !undoAbilityWaiter.Task.IsCompleted)
        {
            undoAbilityWaiter.SetResult(true);
        }

        //Check if all moves in undo is completed
        HistoryStackIsEmpty();
    }

    #region Enabling and Disabling Character
    public void EnableObject()
    {
        _meshRenderer.gameObject.SetActive(true);
        _collider.enabled = true;

        _currentTile.UpdateOccupiedStatus(true, gameObject);
    }

    public void DisableObject()
    {
        _meshRenderer.gameObject.SetActive(false);
        _collider.enabled = false;

        _currentTile.UpdateOccupiedStatus(false, gameObject);
    }
    #endregion

    #region Character Move Ghosting Section

    public void SetGhosting(bool _value)
    {
        if (_ghosting == null) return;

        //GhostManager.isToggled = _value;
        _ghosting.SetGhosting(_value);

        if (_value)
        {
            CharacterReskinner.ShowOutline();
        }
        else
        {
            CharacterReskinner.HideOutline();
        }
    }

    public void ToggleGhosting()
    {
        if (_ghosting == null) return;

        IsGhosting = !IsGhosting;
        //GhostManager.isToggled = IsGhosting;
        _ghosting.toggleGhosting();

        if (GameInputManager.Instance.IsRightClick)
        {
            //Populate it on ghosting is true
            UIManager.Instance.UpdateInfoPanel(characterData.Data);
        }
    }

    public void ClickedOnJump()
    {
        if (isJumping) { return; }
        isJumping = true;
        _meshRenderer.transform.DOJump(_meshRenderer.transform.position, 1f, 1, 0.5f, false)
            .OnComplete(() =>
            {
                isJumping = false;
            });
    }

    #endregion

    #region Character Bark System

    protected bool OnValidateBark()
    {
        if (_characterBark == null)
        {
            Debug.LogError("Character is missing character bark", gameObject);
            return false;
        }

        return true;
    }

    public void OnClickBark()
    {
        if (!OnValidateBark()) return;

        var bark = _characterBark.GetRandomBark(BarkTag.OnClick);

        Debug.Log($"{gameObject.name}: {bark}");

        _characterUI.UpdateBark(bark);
    }

    /// <summary>
    /// The function would be overridden for Redirect wizard
    /// </summary>
    public virtual void OnCastBark()
    {
        if (!OnValidateBark()) return;

        var bark = _characterBark.GetRandomBark(BarkTag.OnCast);

        _characterUI.UpdateBark(bark);
    }

    #endregion
}
