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
        if (other.TryGetComponent<ChRedirectWizard>(out var redirectWizard))
        {
            if (redirectWizard.hasStoredProj)
            {
                _projectileHandle.characterInteracted = redirectWizard;
                _projectileHandle.obstacleInteracted = null;
                _projectileHandle.beforeTileID = redirectWizard.CurrentTileID;
            }
            else
            {
                _projectileHandle = null;
            }
            base.OnTriggerEnter(other);
            return;
        }

        if (other.CompareTag(MetaConstants.CharacterTag))
        {
            var hitCharacter = other.GetComponent<Base_Ch>();
            _projectileHandle.characterInteracted = hitCharacter;
            _projectileHandle.obstacleInteracted = null;
            _projectileHandle.beforeTileID = hitCharacter.CurrentTileID;
            hitCharacter.HitByProjectile(_projectileType);
        }
        if (other.CompareTag(MetaConstants.EnvironmentTag) && other.TryGetComponent<Base_Obstacle>(out var Obstacle))
        {
            _projectileHandle.characterInteracted = null;
            _projectileHandle.obstacleInteracted = Obstacle;
            _projectileHandle.beforeTileID = Obstacle.CurrentTileID;

            other.gameObject.GetComponent<Base_Obstacle>().DisableObject();
            if (other.gameObject.GetComponent<ExplosiveObject>())
            {
                HitExplosive(other);
                VisuallyDestroy();
                //GameTurnManager.Instance.AddDestroyedObject(other.gameObject);
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
