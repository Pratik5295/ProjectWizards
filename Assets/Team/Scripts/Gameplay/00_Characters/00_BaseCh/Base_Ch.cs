using System.Collections;
using System.Collections.Generic;
using Team.Gameplay.GridSystem;
using Team.Enum.Character;
using UnityEngine;

[System.Serializable]
public class PlayerMove
{
    public bool wasMoved;

    public PlayerMove(bool _move)
    {
        wasMoved = _move;
    }
}

[DefaultExecutionOrder(2)]
public class Base_Ch : MonoBehaviour, IMoveable, IProjectileHittable, IUsableAbility, IDestroyable
{
    [Header("Enumerations")]
    [SerializeField] private Enum_CharacterState CharState = Enum_CharacterState.Alive;

    public bool IsAlive => CharState == Enum_CharacterState.Alive;

    public Stack<PlayerMove> HistoryStack = new Stack<PlayerMove>();


    [Header("Script References")]
    [SerializeField] protected GridManager ref_gridManager;

    [SerializeField] protected Base_Rotation baseRotation;
    public Base_Rotation BaseRotation
    {
        get { return baseRotation; }
    }



    [Header("Movement Variables")]
    [Header("---Tile Variables---")]
    [SerializeField] protected TileID _currentTileID = new TileID(0, 0);
    protected TileID _previousTileID = new TileID(0, 0);
    [SerializeField] protected TileID _startTileID = new TileID(0, 0);
    public TileID CurrentTileID
    {
        get { return _currentTileID; }
    }
    private GridTile currentTile;


    private float OffsetValue;
    private float smoothingTime = 1f; //Time to reach the target position.
    private float currentTime; //Current elapsed Time for movement lerp.
    private float lerpingDelayTime = 0.001f;
    [Header("Y Offset and Movement Jump Variables")]
    [SerializeField]
    private float ySpawnOffset = 1.5f;
    [SerializeField]
    private float ydefaultOffset = 1.5f;

    [SerializeField] private AnimationCurve _yMovementCurve;

    private int movementIteration;

    private Vector3 startPosition;

    private bool alreadyMoving;

    #region Vars_InvalidMovementShake

    private float shakeTimer = 0f;

    private float maxShakeAmount = 0.3f;
    #endregion

    #region Mesh And Collider
    private Collider _collider;
    private MeshRenderer _meshRenderer;
    #endregion


    public System.Action OnStateChanged;

    public System.Action OnTurnComplete;

    [ContextMenu("Initialise this Character")]
    public virtual void InitialiseCharacter(TileID StartingTileID, Enum_GridDirection startingDirection)
    {
        ref_gridManager = GridManager.Instance;
        OffsetValue = ref_gridManager.GridSlot_Offset;

        _currentTileID = StartingTileID;
        _previousTileID = _currentTileID;
        _startTileID = _currentTileID;

        currentTile = ref_gridManager.FindTile(_currentTileID);
        currentTile.SetObjectOccupyingTile(this.gameObject);

        baseRotation = GetComponent<Base_Rotation>();
        baseRotation.changeFacingDirection(startingDirection);
        Vector2 v2Dir = baseRotation.dirToV2(baseRotation.DirectionFacing);
        baseRotation.RotateToFaceDir(v2Dir);

        transform.position = new Vector3(currentTile.TilePosition.x, currentTile.TilePosition.y + ySpawnOffset, currentTile.TilePosition.z);

        _collider = GetComponent<Collider>();
        _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    void Start()
    {
        //InitialiseCharacter(_currentTileID, Enum_GridDirection.NORTH);
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
        currentTile.SetObjectOccupyingTile(null);

        for (int i = 0; i < movementAmount; i++)
        {
            alreadyMoving = true;
            Vector3 desiredLocation = new Vector3(_currentTileID.x + (dir.x * OffsetValue), transform.position.y, _currentTileID.y + (dir.y * OffsetValue));

            TileID desiredTileID = new TileID(_currentTileID.x + (int)dir.x, _currentTileID.y + (int)dir.y);
            GridTile targetTile = ref_gridManager.FindTile(desiredTileID);

            if (targetTile && targetTile.IsTileWalkable())
            {
                Vector3 targetPosition = new Vector3(targetTile.TilePosition.x, desiredLocation.y, targetTile.TilePosition.z);

                _currentTileID = targetTile.TileID;
                currentTile = ref_gridManager.FindTile(_currentTileID);

                yield return StartCoroutine(LerpingMovement(targetPosition, wasPushed));
            }
            else
            {
                StartCoroutine(ShakeCharacter(0.25f));
                alreadyMoving = false;
                OnTurnComplete?.Invoke();
                yield break;
            }
        }
        currentTile.SetObjectOccupyingTile(this.gameObject);

        PlayerMove playerMove = new PlayerMove(true);
        HistoryStack.Push(playerMove);

        if (wasPushed) { yield break; }

        OnTurnComplete?.Invoke();
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
            //transform.position = Vector3.Lerp(positionYLerped, targetPosition, lerpAmount);

            yield return null;
        }

        if (currentTime >= smoothingTime)
        {
            currentTime = 0;
            transform.position = targetPosition;
            startPosition = transform.position;

        }

        alreadyMoving = false;
    }

    [ContextMenu("Undo Movement")]
    public virtual void UndoAction()
    {
        while (HistoryStack.Count > 0)
        {
            var move = HistoryStack.Pop();

            if (move.wasMoved)
            {
                UndoMovement();
            }
        }

        OnTurnComplete?.Invoke();
    }

    protected void UndoMovement()
    {
        if (_currentTileID == _startTileID) { return; }
        currentTile.SetObjectOccupyingTile(null);

        _currentTileID = _startTileID;
        currentTile = ref_gridManager.FindTile(_currentTileID);

        currentTile.SetObjectOccupyingTile(this.gameObject);

        transform.position = new Vector3(currentTile.TilePosition.x, transform.position.y, currentTile.TilePosition.z);
    }

    //Shakes character if path or tile is invalid.
    private IEnumerator ShakeCharacter(float MaxShakeTime)
    {
        Vector3 defaultPos = transform.localPosition;

        shakeTimer = 0f;
        
        while(shakeTimer < MaxShakeTime)
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

    public void UpdateCurrentTileID()
    {
        _currentTileID = currentTile.TileID;
    }

    public void SetCurrentTile(TileID updatedTileID, GridTile updatedGridTile)
    {
        _currentTileID = updatedTileID;
        currentTile = updatedGridTile;
    }



    public virtual void HitByProjectile(Enum_ProjectileType projectileType)
    {
        switch (projectileType)
        {
            case Enum_ProjectileType.Fireball:
                CharState = Enum_CharacterState.Dead;

                DisableObject();
                break;
            case Enum_ProjectileType.NonLethalRound:
                CharState = Enum_CharacterState.Incapacitated;
                break;
        }
        OnStateChanged?.Invoke();
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


    public virtual void UseAbility()
    {
        // Debug.LogError($" {gameObject.name} Ability not programmed for character");
        StartCoroutine(MoveByAmount(1, baseRotation.GetFacingDirection()));
    }

    #region Enabling and Disabling Character
    public void EnableObject()
    {
        _meshRenderer.enabled = true;
        _collider.enabled = true;
    }

    public void DisableObject()
    {
        _meshRenderer.enabled = false;
        _collider.enabled = false;
    }
    #endregion
}
