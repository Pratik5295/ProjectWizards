using UnityEngine;
using Team.Enum.Character;
using Team.Managers;
using static Team.GameConstants.MetaConstants;
public class ChRedirectWizard : ChProjectileWizard
{
    private Enum_ProjectileType cachedProjectileType;

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
                else _projectilePrefab = PrefabReference;
                    break;

            case Enum_ProjectileType.NonLethalRound:
                if (_projectilePrefab)
                {
                    Vector2 direction = baseRotation.dirToV2(ProjectileDir);
                    StartCoroutine(MoveByAmount(MoveAmount, direction, true));
                    return;
                }
                else _projectilePrefab = PrefabReference;
                break;
        }

        cachedProjectileType = ProjectileType;
    }


    public override void UndoAction()
    {
        while (HistoryStack.Count > 0)
        {
            var move = HistoryStack.Pop();

            if (move.wasMoved)
            {
                UndoMovement();
            }
            else
            {
                UnreferenceProjectile();
            }
        }

        UnreferenceProjectile();
        OnTurnComplete?.Invoke();
    }

    private void UnreferenceProjectile()
    {
        _projectilePrefab = null;
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
