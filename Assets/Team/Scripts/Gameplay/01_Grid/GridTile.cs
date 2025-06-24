using UnityEngine;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using UnityEditor;


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
        private LevelTileCreator gridManager;

        [SerializeField]
        private GameObject tileObject = null; //The created tile object

        [SerializeField] private GameObject _startingObject;

        [SerializeField] private UITile tileUI;

        [SerializeField]
        private GameObject objectOccupyingTile;
        public GameObject ObjectOccupyingTile
        {
            get { return objectOccupyingTile; }
        }

        /// <summary>
        /// Initialize the tile
        /// </summary>
        public bool Init(LevelTileCreator _tileCreator, TileID _tileId)
        {
            gridManager = _tileCreator;
            TileID = _tileId;

            if (objectOccupyingTile)
            {
                objectOccupyingTile.GetComponent<Base_Obstacle>().UpdateObstacleTileData(TileID, this);
            }

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

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }

        [ContextMenu("Set Tile to Object")]
        public void SetTileObject()
        {
            tileType = TileType.TILE;
            tileObject = SpawnTileObject();

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }

        [ContextMenu("Spawn Object Occupying Tile space")]
        public void SpawnObjectOnTile()
        {
            if (!tileObject) { SetTileObject(); }
            if (isTileOccupied() || canSpawnAnyObject()) { return; }

            tileType = TileType.OCCUPIEDTILE;
            Vector3 spawnLocation = new Vector3(tileObject.transform.position.x, 1.5f, tileObject.transform.position.z);
            Base_Obstacle obstacleData = null;

            if (_startingObject)
            {
                GameObject InstantiatedObject = Instantiate(_startingObject, spawnLocation, Quaternion.identity, tileObject.transform);
                objectOccupyingTile = InstantiatedObject;
                if (!InstantiatedObject.GetComponent<Collider>())
                {
                    InstantiatedObject.AddComponent<BoxCollider>();
                }
                if (!InstantiatedObject.GetComponent<Base_Obstacle>()) 
                {
                    obstacleData = InstantiatedObject.AddComponent<Base_Obstacle>();
                }
                else
                    obstacleData = InstantiatedObject.GetComponent<Base_Obstacle>();
                obstacleData.UpdateObstacleTileData(TileID, this);
                obstacleData.InitialiseObstacleData();
            }
            else
            {
                objectOccupyingTile = Instantiate(gridManager.DefaultObstacle, spawnLocation, Quaternion.identity, tileObject.transform);
                if (!objectOccupyingTile.GetComponent<Collider>())
                {
                    objectOccupyingTile.AddComponent<BoxCollider>();
                }
                if (!objectOccupyingTile.GetComponent<Base_Obstacle>())
                {
                    objectOccupyingTile.AddComponent<Base_Obstacle>();
                }
                objectOccupyingTile.GetComponent<Base_Obstacle>().UpdateObstacleTileData(TileID, this);
                objectOccupyingTile.GetComponent<Base_Obstacle>().InitialiseObstacleData();
            }
            objectOccupyingTile.tag = GameConstants.MetaConstants.EnvironmentTag;

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }

        [ContextMenu("Re-Update Obstacle Data")]
        public void UpdateObstacleData()
        {
            if (!objectOccupyingTile) { return; }
            objectOccupyingTile.GetComponent<Base_Obstacle>().UpdateObstacleTileData(TileID, this);
            objectOccupyingTile.GetComponent<Base_Obstacle>().InitialiseObstacleData();

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }


        private bool isTileOccupied()
        {
            return objectOccupyingTile && tileObject.transform.childCount > 0;
        }

        private bool canSpawnAnyObject()
        {
            return !gridManager.DefaultObstacle && !_startingObject;
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

        public void SetTileType(TileType typeOfTile)
        {
            tileType = typeOfTile;
        }

        public void ShowTileUI()
        {
            tileUI.gameObject.SetActive(true);

            string tileID = $"{TileID.x}, {TileID.y}";

            tileUI.PopulateTileText(tileID);
        }
        public GridTile[] FindNeighbouringTiles()
        {
            GridManager gridInstance = GridManager.Instance;

            GridTile[] NeighbourTiles = new GridTile[5];

            NeighbourTiles[0] = this;
            NeighbourTiles[1] = gridInstance.FindTile(new TileID(TileID.x, TileID.y + 1));
            NeighbourTiles[2] = gridInstance.FindTile(new TileID(TileID.x, TileID.y - 1));
            NeighbourTiles[3] = gridInstance.FindTile(new TileID(TileID.x + 1, TileID.y));
            NeighbourTiles[4] = gridInstance.FindTile(new TileID(TileID.x - 1, TileID.y));

            return NeighbourTiles;
        }

        public void HideTileUI()
        {
            tileUI.gameObject.SetActive(false); 
        }
    }
}
