using UnityEngine;
using Team.Enum.Character;
using Team.Managers;
public class ChRedirectWizard : ChProjectileWizard
{

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

}
