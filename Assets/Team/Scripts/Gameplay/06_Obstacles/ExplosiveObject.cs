using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

public class ExplosiveObject : MoveableObstacle
{
    public FireballProjectile RefFireballProjectile;

    [SerializeField] private GameObject _explosionEffect;
    private VFXManager _explosionVFX;

    [SerializeField] private float ExplosionDelayTime = 1f;

    bool hasFireStarted = false;

    public override void InitialiseObstacle(TileID StartingTileID, Enum_GridDirection startingDirection)
    {
        base.InitialiseObstacle(StartingTileID, startingDirection);

        _explosionVFX = _explosionEffect.GetComponent<VFXManager>();
    }

    public override void DisableObject()
    {
        if (!canBeDestroyed) { return; }

        MakeTileUnwalkable();
        StartCoroutine(Explode());
        isDestroyed = true;
    }

    public override void ResetToStart()
    {
        base.ResetToStart();

        hasFireStarted = false;
    }

    protected IEnumerator Explode()
    {
        //Start Animation Here!
        yield return new WaitForSeconds(ExplosionDelayTime);

        _collider.enabled = false;
        _meshRenderer.enabled = false;

        _explosionVFX.EnableParticleEffectChildren();
        Camera.main.GetComponent<StressReceiver>().InduceStress(0.1f);
        PostProcessManager.instance.Explode();

        AffectTiles();
    }

    protected void AffectTiles()
    {
        GridTile[] NeighbourTiles = _currentGridTile.FindNeighbouringTiles();
        List<OilTile> fireOriginTiles = new List<OilTile>();

        for(int i = 1; i < NeighbourTiles.Length; i++)
        {
            if (NeighbourTiles[i])
            {
                if (NeighbourTiles[i].IsOilTile())
                {
                    if (NeighbourTiles[i].TryGetComponent(out OilTile oilTile) && !oilTile.isOnFire)
                    {
                        fireOriginTiles.Add(oilTile);
                        hasFireStarted = true;
                    }
                }
                if (NeighbourTiles[i].IsIceTile())
                {
                    NeighbourTiles[i].tileType = TileType.TILE;
                    NeighbourTiles[i].hasChangedType = true;
                    NeighbourTiles[i].SpawnTileType();
                    GameTurnManager.Instance.AddChangedTile(NeighbourTiles[i]);
                }

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

        if (hasFireStarted)
        {
            FireSpread.Instance.FireballRef = RefFireballProjectile;
            FireSpread.Instance.StartFire(fireOriginTiles);
            return;
        }
        RefFireballProjectile.CleanUp();
    }

}
