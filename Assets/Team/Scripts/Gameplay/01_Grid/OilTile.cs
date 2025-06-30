using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

public class OilTile : GridTile
{
    public GameObject oilTilePrefab;
    [SerializeField] private GameObject _fireVFXPrefab;

    private VFXManager _fireVFXRef;

    private bool isOnFire = false;

    public override bool Init(LevelTileCreator _tileCreator, TileID _tileId, bool specificType = false)
    {
        gridManager = _tileCreator;
        TileID = _tileId;

        if (objectOccupyingTile)
        {
            objectOccupyingTile.GetComponent<Base_Obstacle>().UpdateObstacleTileData(TileID, this);
        }

        startingTileType = tileType;

        //Check if spawn tile
        if (IsTileWalkable())
        {
            tileObject = SpawnOilTileObject();

            //Setup each tile to be facing north at start
            Direction = new TileDirection
            {
                TileFacing = TileFacing.NORTH
            };

            //Assuming all oil tiles are walkable?
            return true;
        }

       

        return false;
    }

    private GameObject SpawnOilTileObject()
    {
        return Instantiate(oilTilePrefab, transform);
    }

    public void Ignite()
    {
        _fireVFXRef = Instantiate(_fireVFXPrefab, transform).GetComponent<VFXManager>();
        _fireVFXRef.EnableParticleEffectChildren();

        isOnFire = true;

        if (ObjectOccupyingTile.CompareTag(MetaConstants.CharacterTag))
        {
            ObjectOccupyingTile.GetComponent<Base_Ch>().HitByProjectile(Enum_ProjectileType.Fireball);
            GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
        }
        if (ObstacleImplementsScript())
        {
            Base_Obstacle baseObstacle = ObstacleImplementsScript();
            if (baseObstacle.CanBeDestroyed) 
            {
                baseObstacle.DisableObject();
                GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
            }

        }
    }

    private void KillWizardOnTile()
    {

    }

}
