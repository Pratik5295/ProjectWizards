using UnityEngine;
using Team.MetaConstants;

public class FireballProjectile : Base_Projectile
{

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CastingWizard || other.gameObject.layer == 3) { return; } //Check that the collision isnt with the wizard that casted the projectile.

        if (other.CompareTag(MetaConstants.CharacterTag))
        {
            other.GetComponent<Base_Ch>().HitByProjectile(_projectileType);
        }
        base.OnTriggerEnter(other);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }
}
