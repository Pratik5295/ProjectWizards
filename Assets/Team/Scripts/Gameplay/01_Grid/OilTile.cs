using System.Collections;
using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.GameLevelSystem;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

public class OilTile : GridTile
{
    public GameObject oilTilePrefab;
    [SerializeField] private GameObject _fireVFXPrefab;

    private GameObject _fireVFXRef;

    [SerializeField]
    private GameObject _oilSpillMesh;

    public bool isOnFire = false;
    public bool visited = false;

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

            _oilSpillMesh = tileObject.transform.GetChild(1).gameObject;
            //Assuming all oil tiles are walkable?
            return true;
        }

       

        return false;
    }

    private void Awake()
    {
        if (!_oilSpillMesh)
        {
            _oilSpillMesh = tileObject.transform.GetChild(1).gameObject;
        }

        //Do not include anything that is relient on the level having been loaded first!
        _startingTileID = TileID;
        _startingObjectInstance = objectOccupyingTile;

        _startingMaterial = tileObject.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        normalColour = _startingMaterial.color;
}

    private GameObject SpawnOilTileObject()
    {
        return Instantiate(oilTilePrefab, transform);
    }

    public void Ignite()
    {
        _fireVFXRef = Instantiate(_fireVFXPrefab, transform);
        //_fireVFXRef.EnableParticleEffectChildren();

        isOnFire = true;
        if (ObjectOccupyingTile)
        {
            if (ObjectOccupyingTile.CompareTag(MetaConstants.CharacterTag))
            {
                KillWizardOnTile();
            }
            if (ObstacleImplementsScript())
            {
                DestroyObjectOnTile();
            }
        }
    }

    private void KillWizardOnTile()
    {
        ObjectOccupyingTile.GetComponent<Base_Ch>().HitByProjectile(Enum_ProjectileType.Fireball);
        GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
    }

    private void DestroyObjectOnTile()
    {
        Base_Obstacle baseObstacle = ObstacleImplementsScript();
        if (baseObstacle.CanBeDestroyed)
        {
            if (baseObstacle.isDestroyed) { return; }
            baseObstacle.DisableObject();
            GameTurnManager.Instance.AddDestroyedObject(ObjectOccupyingTile);
        }
    }

    public void Extinguish()
    {
        isOnFire = false;

        if (_oilSpillMesh)
        {
            _oilSpillMesh.SetActive(false);
        }
        tileObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.grey;
        Destroy(_fireVFXRef);
    }

    public void ResetOilStatus()
    {
        isOnFire = false;
        visited = false;

        if (_oilSpillMesh)
        {
            _oilSpillMesh.SetActive(true);
        }
        tileObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;

        Destroy(_fireVFXRef);
    }



}
