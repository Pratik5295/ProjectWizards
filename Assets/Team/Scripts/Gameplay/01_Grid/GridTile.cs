using UnityEngine;
using Team.MetaConstants;


namespace Team.Gameplay.GridSystem
{
    public enum TileType
    {
        EMPTY = 0, //Highlights no tile
        TILE = 1, //Tile has content (not an empty tile)
        OCCUPIEDTILE = 2 //Tile contains object
    }

    public enum TileFacing
    {
        NORTH = 0,
        SOUTH = 1,
        EAST = 2,   
        WEST = 3
    }

    [System.Serializable]
    public class TileDirection
    {
        public TileFacing TileFacing;
    }

    public class GridTile : MonoBehaviour
    {
        public TileID TileID; // ID of the tile in the grid

        public Vector3 TilePosition => transform.position;

        public GameObject tilePrefab;

        public TileType tileType;

        public TileDirection Direction; //Rotation 

        [SerializeField]
        private GridManager gridManager;

        [SerializeField]
        private GameObject tileObject = null; //The created tile object

        [SerializeField] private GameObject _startingObject;

        private GameObject objectOccupyingTile;
        public GameObject ObjectOccupyingTile
        {
            get { return objectOccupyingTile; }
        }

        /// <summary>
        /// Initialize the tile
        /// </summary>
        public bool Init(GridManager _gridManager, TileID _tileId)
        {
            gridManager = _gridManager;

            TileID = _tileId;

            //Check if spawn tile
            if (IsTileWalkable())
            {
                tileObject = SpawnTileObject();

                //Setup each tile to be facing north at start
                Direction = new TileDirection
                {
                    TileFacing = TileFacing.NORTH
                };


                return true;
            }

            return false;
        }

        private GameObject SpawnTileObject()
        {
            return Instantiate(tilePrefab, transform);
        }

        public bool IsTileWalkable()
        {
            return tileType == TileType.TILE;
        }

        [ContextMenu("Set Tile Empty")]
        public void SetTileEmpty()
        {
            tileType = TileType.EMPTY;
            DestroyImmediate(tileObject);
            tileObject = null;
            gridManager?.RemoveTileFromGrid(TileID, this);
        }

        [ContextMenu("Set Tile to Object")]
        public void SetTileObject()
        {
            tileType = TileType.TILE;
            tileObject = SpawnTileObject();
            gridManager?.AddTileToGrid(TileID, this);
        }

        [ContextMenu("Spawn Object Occupying Tile space")]
        public void SpawnObjectOnTile()
        {
            if (!tileObject) { SetTileObject(); }
            if (isTileOccupied() || !gridManager.DefaultObstacle) { return; }
            tileType = TileType.OCCUPIEDTILE;
            Vector3 spawnLocation = new Vector3(tileObject.transform.position.x, 1.5f, tileObject.transform.position.z);
            if (_startingObject)
            {
                GameObject InstantiatedObject = Instantiate(_startingObject, spawnLocation, Quaternion.identity, tileObject.transform);
                objectOccupyingTile = InstantiatedObject;
                if (!InstantiatedObject.GetComponent<Collider>())
                {
                    InstantiatedObject.AddComponent<BoxCollider>();
                }
                if (!InstantiatedObject.GetComponent<ObstacleData>()) 
                {
                    ObstacleData obstacleData = InstantiatedObject.AddComponent<ObstacleData>();
                    obstacleData.UpdateObstacleTileData(TileID, this);
                }
            }
            else
            {
                objectOccupyingTile = Instantiate(gridManager.DefaultObstacle, spawnLocation, Quaternion.identity, tileObject.transform);
                if (!objectOccupyingTile.GetComponent<Collider>())
                {
                    objectOccupyingTile.AddComponent<BoxCollider>();
                }
                if (!objectOccupyingTile.GetComponent<ObstacleData>())
                {
                    objectOccupyingTile.AddComponent<ObstacleData>();
                }
                objectOccupyingTile.GetComponent<ObstacleData>().UpdateObstacleTileData(TileID, this);
            }
            objectOccupyingTile.tag = MetaConstants.MetaConstants.EnvironmentTag;
        }

        private bool isTileOccupied()
        {
            return objectOccupyingTile && tileObject.transform.childCount > 0;
        }

        public void SetObjectOccupyingTile(GameObject Object)
        {
            if (!Object) { objectOccupyingTile = null; }
            objectOccupyingTile = Object;
        }

        public void ParentOccupyingObject()
        {
            if (!objectOccupyingTile.CompareTag("Character")) { return; }
            objectOccupyingTile.transform.SetParent(transform);

        }

        public void UnparentOccupyingObject()
        {
            if (!objectOccupyingTile.CompareTag("Character")) { return; }
            objectOccupyingTile.transform.SetParent(null);
        }
    }
}
