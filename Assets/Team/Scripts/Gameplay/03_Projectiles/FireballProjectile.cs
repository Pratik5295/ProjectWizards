using UnityEngine;

public class FireballProjectile : Base_Projectile
{

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CastingWizard || other.gameObject.layer == 3) { return; } //Check that the collision isnt with the wizard that casted the projectile.

        if (other.CompareTag("Character"))
        {
            other.GetComponent<Base_Ch>().HitByProjectile(_projectileType);
        }
        if (_collisionEffect) { _collisionEffect.Play();}

        Destroy(this.gameObject);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }
}
