using UnityEngine;
using Team.MetaConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;

public class FireballProjectile : Base_Projectile
{

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CastingWizard || other.gameObject.layer == 3) { return; } //Check that the collision isnt with the wizard that casted the projectile.

        if (other.CompareTag(MetaConstants.CharacterTag))
        {
            other.GetComponent<Base_Ch>().HitByProjectile(_projectileType);
            other.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
            //Needs to reset when restarting.
        }
        if (other.CompareTag(MetaConstants.EnvironmentTag))
        {
            other.gameObject.GetComponent<ObstacleData>().MakeTileWalkable();
            if (!other.GetComponent<MeshRenderer>())
            {
                other.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
            else
            {
                other.GetComponent<MeshRenderer>().enabled = false;
            }

        }
        GameTurnManager.Instance.AddDestroyedObject(other.gameObject);

        base.OnTriggerEnter(other);
    }

    public override void CleanUp()
    {
        base.CleanUp();
    }
}
