using System;
using System.Collections;
using UnityEngine;
using Team.Gameplay.GridSystem;
using Team.GameConstants;
using Team.Managers;

public class PushProjectile : Base_Projectile
{
    [Tooltip("Amount of tiles the character should be pushed, from the projectiles collision position.")]
    [SerializeField] private float _pushAmount = 1f;

    public override void FiredSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.s_push, this.transform.position);
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
            Base_Ch characterScript = other.gameObject.GetComponent<Base_Ch>();
            Vector2 direction = characterScript.BaseRotation.dirToV2(_projectileDir);

            PushedSomething(true, characterScript, null);
            characterScript.StartCoroutine(characterScript.MoveByAmount((int)_pushAmount, direction, true));

            return;
        }
        else if (other.CompareTag(MetaConstants.EnvironmentTag) && other.GetComponent<MoveableObstacle>())
        {
            MoveableObstacle ObstacleScript = other.gameObject.GetComponent<MoveableObstacle>();
            Vector2 direction = ObstacleScript.BaseRotation.dirToV2(_projectileDir);

            PushedSomething(false, null, ObstacleScript);
            ObstacleScript.StartCoroutine(ObstacleScript.MoveByAmount((int)_pushAmount, direction, true));

            return;
        }
        base.OnTriggerEnter(other);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }

    public void PushedSomething(bool PushingCharacter, Base_Ch BaseCh = null, MoveableObstacle Obstacle = null)
    {
        VisuallyDestroy();
        if (PushingCharacter)
        {
            if (BaseCh)
            {
                BaseCh.PushProjectileInstance = this;
            }
            else 
            { 
                Debug.LogError("Wasnt a character I pushed!"); 
            }
        }
        else if (!PushingCharacter)
        {
            if (Obstacle)
            {
                Obstacle.PushProjectileInstance = this;
            }
            else 
            {
                Debug.LogError("Cant Push this obstacle!");
            }
        }
    }
}
