using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

public class ExplosiveObject : MoveableObstacle
{

    [SerializeField] private GameObject _explosionEffect;
    private VFXManager _explosionVFX;

    public override void InitialiseObstacle(TileID StartingTileID, Enum_GridDirection startingDirection)
    {
        base.InitialiseObstacle(StartingTileID, startingDirection);

        _explosionVFX = _explosionEffect.GetComponent<VFXManager>();
    }

    public override void DisableObject()
    {
        base.DisableObject();
        Explode();
    }

    protected void Explode()
    {
        //Cannot find VFX manager.
        _explosionVFX.EnableParticleEffectChildren();

        AffectTiles();
    }

    protected void AffectTiles()
    {
        GridTile[] NeighbourTiles = _currentGridTile.FindNeighbouringTiles();

        for(int i = 1; i < NeighbourTiles.Length; i++)
        {
            if (!NeighbourTiles[i].ObjectOccupyingTile) { continue; }
            GameObject ObjectOccupyingTile = NeighbourTiles[i].ObjectOccupyingTile;

            if (ObjectOccupyingTile.CompareTag(MetaConstants.CharacterTag))
            {
                ObjectOccupyingTile.GetComponent<Base_Ch>().HitByProjectile(Enum_ProjectileType.Fireball);
            }
            if (ObjectOccupyingTile.CompareTag(MetaConstants.EnvironmentTag))
            {
                ObjectOccupyingTile.GetComponent<Base_Obstacle>().DisableObject();
            }
            GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
        }
    }

}
