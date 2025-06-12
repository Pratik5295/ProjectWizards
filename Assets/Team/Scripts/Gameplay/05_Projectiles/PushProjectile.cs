using System;
using System.Collections;
using UnityEngine;
using Team.Gameplay.GridSystem;
using Team.GameConstants;

public class PushProjectile : Base_Projectile
{
    [Tooltip("Amount of tiles the character should be pushed, from the projectiles collision position.")]
    [SerializeField] private float _pushAmount = 1f;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == CastingWizard || other.gameObject.layer == 3) { return; } //Check that the collision isnt with the wizard that casted the projectile.

        if (other.CompareTag(MetaConstants.CharacterTag))
        {
            Base_Ch characterScript = other.gameObject.GetComponent<Base_Ch>();
            Vector2 direction = characterScript.BaseRotation.dirToV2(_projectileDir);
            characterScript.StartCoroutine(characterScript.MoveByAmount((int)_pushAmount, direction, true));
        }
        else if (other.CompareTag(MetaConstants.EnvironmentTag) && other.GetComponent<Base_Ch>())
        {
            Base_Ch characterScript = other.gameObject.GetComponent<Base_Ch>();
            Vector2 direction = characterScript.BaseRotation.dirToV2(_projectileDir);
            characterScript.StartCoroutine(characterScript.MoveByAmount((int)_pushAmount, direction, true));
        }
        base.OnTriggerEnter(other);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }
}
