using UnityEngine;
using Team.MetaConstants;
using Team.Gameplay.GridSystem;

public class FireballProjectile : Base_Projectile
{

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CastingWizard || other.gameObject.layer == 3) { return; } //Check that the collision isnt with the wizard that casted the projectile.

        if (other.CompareTag(MetaConstants.CharacterTag))
        {
            other.GetComponent<Base_Ch>().HitByProjectile(_projectileType);
        }
        if (other.CompareTag(MetaConstants.EnvironmentTag))
        {
            //other.gameObject.GetComponent<ObstacleData>().clearTileData();
            Destroy(other.gameObject);
        }
        base.OnTriggerEnter(other);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }
}
