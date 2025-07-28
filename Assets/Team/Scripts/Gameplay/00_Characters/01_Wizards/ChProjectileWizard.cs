using System.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class ChProjectileWizard : Base_Ch
{
    [Header("Projectile Wizard Variables")]

    [Header("---Object References---")]
    [SerializeField] protected GameObject _projectilePrefab;
    public GameObject ProjectilePrefab
    {
        set { _projectilePrefab = value; }
        get { return _projectilePrefab; }
    }
    [SerializeField] private GameObject _fireFromPoint;

    private GameObject ProjectileInstance;
    private Base_Projectile scProjectile;
    private QuadraticCurve curve;

    public ProjectileGhosting ghostingEffect;

    [Header("---Wizard Projectile Stats---")]
    [Range(-1, -300)]
    [SerializeField] private int _projectileRange = -1;
    

    void Awake()
    {
        curve = _fireFromPoint.GetComponent<QuadraticCurve>();

        curve.startPoint = new GameObject("Curve_StartPoint").transform;
        curve.startPoint.transform.parent = curve.transform;
        curve.startPoint.transform.localPosition = new Vector3(0, 0, 0);

        curve.endPoint = new GameObject("Curve_EndPoint").transform;
        curve.endPoint.transform.parent = curve.transform;
        curve.endPoint.localPosition = new Vector3(_projectileRange, 0, 0);

        curve.controlPoint = new GameObject("Curve_ControlPoint").transform;
        curve.controlPoint.transform.parent = curve.transform;
        curve.controlPoint.localPosition = new Vector3(_projectileRange/2, 0, 0);

        if (transform.GetComponentInChildren<ProjectileGhosting>())
        {
            ghostingEffect = transform.GetComponentInChildren<ProjectileGhosting>();
            ghostingEffect.SetProjectionValue(_projectileRange);
        }
    }


    public async override Task UseAbility()
    {
        abilityWaiter = new TaskCompletionSource<bool>();
        if (_projectilePrefab == null) 
        { 
            StartCoroutine(ShakeCharacter(0.25f)); 
            endTurn(null);
            return;
        }
        ProjectileInstance = Instantiate(_projectilePrefab, _fireFromPoint.transform.position, Quaternion.identity);
        scProjectile = ProjectileInstance.GetComponent<Base_Projectile>();

        scProjectile.curve = curve;
        scProjectile.CastingWizard = this.gameObject;
        scProjectile._prefabReference = _projectilePrefab;
        scProjectile._projectileDir = baseRotation.DirectionFacing;
        scProjectile.OnProjectileEnd += endTurn;

        OnCastBark();

        await abilityWaiter.Task;
    }

    private void endTurn(ProjectileHandler _projectileHandler)
    {
        if (_projectilePrefab != null)
        {
            scProjectile.OnProjectileEnd -= endTurn;
        }
        ProjectileInstance = null;
        scProjectile = null;

        if(_projectileHandler != null)
        {
            //Turn detected

            PlayerMoveType type = PlayerMoveType.PUSH;

            switch (_projectileHandler.ProjectileType)
            {
                case Team.Enum.Character.Enum_ProjectileType.Fireball:
                    type = PlayerMoveType.DESTROYED;
                    break;

                case Team.Enum.Character.Enum_ProjectileType.NonLethalRound:
                    type = PlayerMoveType.PUSH;
                    break;
            }


            PlayerMove playerMove 
                = new PlayerMove
                (_projectileHandler.beforeTileID, type, baseRotation.DirectionFacing,
                _projectileHandler.characterInteracted, _projectileHandler.obstacleInteracted);

            HistoryStack.Push(playerMove);
        }

        OnAbilityCompleted();
    }

    protected override async Task CharacterUndoStack()
    {
        undoAwaiter = new TaskCompletionSource<bool>();

        Debug.Log($"{gameObject.name} Moves count: {HistoryStack.Count}");

        if (HistoryStack.Count == 0)
        {
        }
        else
        {
            while (HistoryStack.Count > 0)
            {
                var move = HistoryStack.Pop();

                //If its a character interacted 

                if (move.interactedWith != null)
                {

                    if (move.Type == PlayerMoveType.PUSH)
                    {
                        move.interactedWith.UndoMovement(this, move);
                    }
                    else if (move.Type == PlayerMoveType.DESTROYED)
                    {
                        UndoDestroy(this,move);
                    }
                }

                else if(move.interactedObstacle != null)
                {
                    if (move.Type == PlayerMoveType.PUSH)
                    {
                        if (move.interactedObstacle.TryGetComponent<MoveableObstacle>(out var moveableObstacle))
                        {
                            moveableObstacle.ForceUndoMovement(this, move.movedFrom);
                        }
                        else
                        {
                            //FUTURE SCARE: Unmoveable obstacles need to fire Undo Complete as they wont move
                            //Need to do the same in redirect wizard
                        }
                    }
                    else if(move.Type == PlayerMoveType.DESTROYED)
                    {
                        UndoDestroyObstacle(this,move); 
                    }
                }
            }
            await undoAwaiter.Task;
        }
    }

    protected void UndoDestroy(Base_Ch _character,PlayerMove _move)
    {
        var ch = _move.interactedWith;

        ch.resetCharState(true);

        _character.OnUndoAbilityCompleted();
    }

    protected void UndoDestroyObstacle(Base_Ch _character, PlayerMove _move)
    {
        var interactedObstacle = _move.interactedObstacle;

        interactedObstacle.ResetToStart();

        _character.OnUndoAbilityCompleted();
    }
}
