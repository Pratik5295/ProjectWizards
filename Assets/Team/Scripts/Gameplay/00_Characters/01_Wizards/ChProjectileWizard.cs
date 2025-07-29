using System.Collections.Generic;
using System.Threading.Tasks;
using Team.GameConstants;
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
        curve.controlPoint.localPosition = new Vector3(_projectileRange / 2, 0, 0);

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

        if (_projectileHandler != null)
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

            //Characters interacted with
            List<CharacterInteraction> characterInteractions = new List<CharacterInteraction>();
            //Obstacles Interacted with

            List<ObstacleInteraction> obstacleInteractions = new List<ObstacleInteraction>();

            //Check if project handler interacted with a character or obstacle

            if(_projectileHandler.characterInteracted == null && _projectileHandler.obstacleInteracted == null)
            {
                Debug.LogWarning("No character or obstacle interacted with, ending turn without recording move.");
                OnAbilityCompleted();
                return;
            }

            if (_projectileHandler.characterInteracted != null 
                && _projectileHandler.characterInteracted.gameObject.CompareTag(MetaConstants.CharacterTag))
            {
                CharacterInteraction interactedCharacter = new CharacterInteraction();

                interactedCharacter.CharacterInteracted = _projectileHandler.characterInteracted;
                interactedCharacter.PreviousTileID = _projectileHandler.beforeTileID;
                interactedCharacter.PreviousDirection = _projectileHandler.characterInteracted.GetComponent<Base_Rotation>().DirectionFacing;
                characterInteractions.Add(interactedCharacter);
            }
            
            if (_projectileHandler.obstacleInteracted != null 
                && _projectileHandler.obstacleInteracted.gameObject.CompareTag(MetaConstants.EnvironmentTag))
            {
                ObstacleInteraction obstacleInteraction = new ObstacleInteraction();

                obstacleInteraction.ObstacleInteracted = _projectileHandler.obstacleInteracted;
                obstacleInteraction.PreviousTileID = _projectileHandler.beforeTileID;
                obstacleInteraction.PreviousDirection = baseRotation.DirectionFacing;

                obstacleInteractions.Add(obstacleInteraction);
            }

            //Update Player Move with new data

            PlayerMove playerMove
                = new PlayerMove
                (type, characterInteractions, obstacleInteractions);

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

                if (move.CharacterInteractions != null)
                {

                    if (move.Type == PlayerMoveType.PUSH)
                    {
                        foreach (var _character in move.CharacterInteractions)
                        {
                            _character.CharacterInteracted.UndoMovement(_character, this);
                        }
                    }
                    else if (move.Type == PlayerMoveType.DESTROYED)
                    {
                        UndoDestroy(this, move);
                    }
                }

                else if (move.ObstaclesInteractions != null)
                {
                    if (move.Type == PlayerMoveType.PUSH)
                    {
                        foreach (var obstacleInteraction in move.ObstaclesInteractions)
                        {
                            if (obstacleInteraction.ObstacleInteracted.TryGetComponent<MoveableObstacle>(out var moveableObstacle))
                            {
                                moveableObstacle.ForceUndoMovement(this, obstacleInteraction.PreviousTileID);
                            }
                            else
                            {
                                //FUTURE SCARE: Unmoveable obstacles need to fire Undo Complete as they wont move
                            }
                        }
                    }
                    else if (move.Type == PlayerMoveType.DESTROYED)
                    {
                        UndoDestroyObstacle(this, move);
                    }
                }
            }
            await undoAwaiter.Task;
        }
    }

    protected void UndoDestroy(Base_Ch _character, PlayerMove _move)
    {
        foreach (var _ch in _move.CharacterInteractions)
        {
            _ch.CharacterInteracted.resetCharState(true);
        }

        _character.OnUndoAbilityCompleted();
    }

    protected void UndoDestroyObstacle(Base_Ch _character, PlayerMove _move)
    {
        foreach (var obstacleInteraction in _move.ObstaclesInteractions)
        {
            obstacleInteraction.ObstacleInteracted.ResetToStart();
        }

        _character.OnUndoAbilityCompleted();
    }
}
