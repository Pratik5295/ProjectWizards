using UnityEngine;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using Unity.VisualScripting;

public class FireballProjectile : Base_Projectile
{

    public override void FiredSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.s_fireball, this.transform.position);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == CastingWizard || other.gameObject.layer == 3) { return; } //Check that the collision isnt with the wizard that casted the projectile.
        if (other.GetComponent<ChRedirectWizard>())
        {
            base.OnTriggerEnter(other);
            return;
        }

        if (other.CompareTag(MetaConstants.CharacterTag))
        {
            other.GetComponent<Base_Ch>().HitByProjectile(_projectileType);
        }
        if (other.CompareTag(MetaConstants.EnvironmentTag) && other.GetComponent<Base_Obstacle>())
        {
            other.gameObject.GetComponent<Base_Obstacle>().DisableObject();
            if (other.gameObject.GetComponent<ExplosiveObject>())
            {
                HitExplosive(other);
                VisuallyDestroy();
                GameTurnManager.Instance.AddDestroyedObject(other.gameObject);
                return;
            }
        }
        GameTurnManager.Instance.AddDestroyedObject(other.gameObject);

        base.OnTriggerEnter(other);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }

    private void HitExplosive(Collider other)
    {
        GameTurnManager.Instance.AddDestroyedObject(other.gameObject);
        other.gameObject.GetComponent<ExplosiveObject>().RefFireballProjectile = this;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
