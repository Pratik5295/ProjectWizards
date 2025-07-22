using UnityEngine;
using Team.Enum.Character;
using Team.Managers;
using static Team.GameConstants.MetaConstants;
using System.Threading.Tasks;

public static partial class RedirectConstants
{
    public static float VFXOffset = 0.5f;
    public static float VFXRotation = 90f;
}
public class ChRedirectWizard : ChProjectileWizard
{
    private Enum_ProjectileType cachedProjectileType;

    [Header("Redirect Variables")]
    [SerializeField]
    private GameObject ShieldVFX;

    public bool hasStoredProj => _projectilePrefab != null;

    public void TryAbsorbProjectile(Enum_ProjectileType ProjectileType, GameObject PrefabReference, Enum_GridDirection ProjectileDir = Enum_GridDirection.NORTH, int MoveAmount = 0)
    {
        if (!PrefabReference)
        {
            Debug.LogError("NO PREFAB REFERENCE - recieved on " + gameObject.name);
            return;
        }
        switch (ProjectileType)
        {
            case Enum_ProjectileType.Fireball:
                if (_projectilePrefab)
                {
                    HitByProjectile(ProjectileType);
                    GameTurnManager.Instance.AddDestroyedObject(gameObject);
                    return;
                }
                else 
                {
                    AbsorbVFX(ProjectileDir);
                    _projectilePrefab = PrefabReference;
                }

                    break;

            case Enum_ProjectileType.NonLethalRound:
                if (_projectilePrefab)
                {
                    Vector2 direction = baseRotation.dirToV2(ProjectileDir);
                    StartCoroutine(MoveByAmount(MoveAmount, direction, true));
                    return;
                }
                else
                {
                    AbsorbVFX(ProjectileDir);
                    _projectilePrefab = PrefabReference;
                }
                break;
        }

        cachedProjectileType = ProjectileType;
    }


    protected override async Task CharacterUndoStack()
    {
        undoAwaiter = new TaskCompletionSource<bool>();
        while (HistoryStack.Count > 0)
        {
            var move = HistoryStack.Pop();

            //If its a character
            if (move.interactedWith != null)
            {
                if (move.Type == PlayerMoveType.PUSH)
                {
                    move.interactedWith.UndoMovement(this, move.movedFrom);
                }
                else if (move.Type == PlayerMoveType.DESTROYED)
                {
                    UndoDestroy(this, move);
                }
                else
                {
                    UnreferenceProjectile();
                }
            }

            //If its an obstacle
            else if (move.interactedObstacle != null)
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
                    }

                }
                else if (move.Type == PlayerMoveType.DESTROYED)
                {
                    move.interactedObstacle.ResetToStart();
                }
            }
        }

        UnreferenceProjectile();

        await undoAwaiter.Task;

        //OnTurnComplete?.Invoke();
    }

    private void UnreferenceProjectile()
    {
        _projectilePrefab = null;

        OnUndoAbilityCompleted();
    }

    private void AbsorbVFX(Enum_GridDirection ProjectileDir = Enum_GridDirection.NORTH)
    {
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        switch (ProjectileDir)
        {
            case Enum_GridDirection.NORTH:
                spawnPosition = new Vector3(0, 0, -RedirectConstants.VFXOffset);
                spawnRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Enum_GridDirection.SOUTH:
                spawnPosition = new Vector3(0, 0, RedirectConstants.VFXOffset);
                spawnRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Enum_GridDirection.WEST:
                spawnPosition = new Vector3(RedirectConstants.VFXOffset, 0, 0);
                spawnRotation = Quaternion.Euler(0, RedirectConstants.VFXRotation, 0);
                break;
            case Enum_GridDirection.EAST:
                spawnPosition = new Vector3(-RedirectConstants.VFXOffset, 0, 0);
                spawnRotation = Quaternion.Euler(0, RedirectConstants.VFXRotation, 0);
                break;
        }
        VFXManager VFX = Instantiate(ShieldVFX, transform.position + spawnPosition, spawnRotation).GetComponent<VFXManager>();
        VFX.transform.parent = transform;
        VFX.EnableParticleEffectChildren();
    }

    public override void OnCastBark()
    {
        if (!OnValidateBark()) return;

        string bark = string.Empty;

        if (_projectilePrefab == null)
        {
            bark = _characterBark.GetRandomBark(BarkTag.OnFailcast);
        }
        else
        {
            //Check what type of projectile are we holding
            switch (cachedProjectileType)
            {
                case Enum_ProjectileType.Fireball:
                    bark = _characterBark.GetRandomBark(BarkTag.OnFirecast); 
                    break;
                case Enum_ProjectileType.NonLethalRound:
                    bark = _characterBark.GetRandomBark(BarkTag.OnWindcast);
                    break;
            }
        }


        _characterUI.UpdateBark(bark);
    }


}
