using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

public class OilTile : GridTile
{

    [SerializeField] private GameObject _fireVFXPrefab;

    private VFXManager _fireVFXRef;

    private bool isOnFire = false;


    public void Ignite()
    {
        _fireVFXRef = Instantiate(_fireVFXPrefab, transform).GetComponent<VFXManager>();
        _fireVFXRef.EnableParticleEffectChildren();

        isOnFire = true;

        if (ObjectOccupyingTile.CompareTag(MetaConstants.CharacterTag))
        {
            ObjectOccupyingTile.GetComponent<Base_Ch>().HitByProjectile(Enum_ProjectileType.Fireball);
            GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
        }
        if (ObstacleImplementsScript())
        {
            Base_Obstacle baseObstacle = ObstacleImplementsScript();
            if (baseObstacle.CanBeDestroyed) 
            {
                baseObstacle.DisableObject();
                GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
            }

        }
    }

    private void KillWizardOnTile()
    {

    }

}
