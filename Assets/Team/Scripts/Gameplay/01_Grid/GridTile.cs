using UnityEngine;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using UnityEditor;
using UnityEngine.Events;
using Team.Gameplay.GameLevelSystem;
using Team.Managers;


namespace Team.Gameplay.GridSystem
{
    public enum TileType
    {
        EMPTY = 0, //Highlights no tile
        TILE = 1, //Tile has content (not an empty tile)
        OCCUPIEDTILE = 2, //Tile contains object
        DEATHTILE = 3, //Tile that should kill player
        ICETILE = 4, //Tile should slide player to next tile in their direction of travel.
        OILTILE = 5 //Tile contains oil which will set fire and start a chain reaction.
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

        public GameObject deathTilePrefab;
        public GameObject iceTilePrefab;

        [SerializeField]
        public TileType tileType = TileType.TILE;
        public TileType myTileType
        {
            get => tileType;
            set
            {
                if (tileType != value)
                {
                    tileType = value;
#if UNITY_EDITOR
                    EditorSpawnTileType();

#else
                    SpawnTileType();
#endif

                }
            }
        }

        [SerializeField]
        protected TileType startingTileType;

        public TileDirection Direction; //Rotation 

        [SerializeField]
        protected LevelTileCreator gridManager;

        [SerializeField]
        protected GameObject tileObject = null; //The created tile object

        [SerializeField] private GameObject _startingObject;

        [SerializeField] private UITile tileUI;

        [SerializeField]
        protected GameObject objectOccupyingTile;
        public GameObject ObjectOccupyingTile
        {
            get { return objectOccupyingTile; }
        }

        /// <summary>
        /// Initialize the tile
        /// </summary>
        public virtual bool Init(LevelTileCreator _tileCreator, TileID _tileId, bool specificType = false)
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
                if (!specificType)
                {
                    tileObject = SpawnTileObject();
                }

                //Setup each tile to be facing north at start
                Direction = new TileDirection
                {
                    TileFacing = TileFacing.NORTH
                };


                return true;
            }

            return false;
        }

        #region Spawning Tile Types
        private GameObject SpawnTileObject()
        {
            return Instantiate(tilePrefab, transform);
        }


        private GameObject SpawnDeathTileObject()
        {
            return Instantiate(deathTilePrefab, transform);
        }
        private GameObject SpawnIceTileObject()
        {
            return Instantiate(iceTilePrefab, transform);
        }

        #endregion

        #region is Tile? Boolean Checks
        public bool IsTileWalkable()
        {
            return tileType == TileType.TILE || tileType == TileType.OILTILE || tileType == TileType.DEATHTILE || tileType == TileType.ICETILE;
        }

        public bool IsOilTile()
        {
            return tileType == TileType.OILTILE;
        }
        public bool IsDeathTile()
        {
            return tileType == TileType.DEATHTILE;
        }

        public bool IsIceTile()
        {
            return tileType == TileType.ICETILE;
        }
        #endregion

        #region Setting Tile Types

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

        [ContextMenu("Set starting tile type")]
        public void SetStartingTileType()
        {
            startingTileType = tileType;
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }
        #endregion

        public virtual void SpawnTileType()
        {
            if (tileObject && tileType != TileType.OCCUPIEDTILE)
            {
                Destroy(tileObject);
                tileObject = null;
            }

            /* if(tileType == TileType.OILTILE) // Cannot do this as this will execute after tile is set to oil.
             {
                 LevelManager.Instance.CreatedLevel.LevelTiles.RemoveTile(this);
                 GridTile newTile = LevelManager.Instance.CreatedLevel.LevelTiles.CreateNewTile(this);
                 newTile.SpawnTileType();
                 Destroy(gameObject);
             }*/

            HandleTileSpawn();
        }

        [ExecuteInEditMode]
        protected virtual void EditorSpawnTileType()
        {
            if (tileObject && tileType != TileType.OCCUPIEDTILE)
            {
#if UNITY_EDITOR
                DestroyImmediate(tileObject);
#endif
                tileObject = null;
            }
            /* if(tileType == TileType.OILTILE) // Cannot do this as this will execute after tile is set to oil.
             {
                 LevelManager.Instance.CreatedLevel.LevelTiles.RemoveTile(this);
                 GridTile newTile = LevelManager.Instance.CreatedLevel.LevelTiles.CreateNewTile(this);
                 newTile.SpawnTileType();
                 Destroy(gameObject);
             }*/

            HandleTileSpawn();
            if (tileType != TileType.OILTILE)
            {
                gridManager.DirtySaveTileChanges();
            }
        }

        private void HandleTileSpawn()
        {
            switch (tileType)
            {
                case TileType.TILE:
                    tileObject = SpawnTileObject();
                    break;

                case TileType.EMPTY:
                    DestroyImmediate(tileObject);
                    tileObject = null;
                    break;

                case TileType.DEATHTILE:
                    tileObject = SpawnDeathTileObject();
                    break;

                case TileType.OILTILE:
                    gridManager.CreateNewOilTile(TileID, transform.position.x, transform.position.z);
                    break;

                case TileType.ICETILE:
                    tileObject = SpawnIceTileObject();
                    break;

                case TileType.OCCUPIEDTILE:
                    SpawnObjectOnTile();
                    break;
            }
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

        public Base_Obstacle ObstacleImplementsScript()
        {
            return objectOccupyingTile.GetComponent<Base_Obstacle>();
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
            SetTileType(TileType.OCCUPIEDTILE);
        }

        /// <summary>
        /// Updates the tiles occupied status.
        /// </summary>
        /// <param name="isOccupied"></param>
        /// <param name="OccupyingObject"></param>
        public void UpdateOccupiedStatus(bool isOccupied = false, GameObject OccupyingObject = null)
        {
            switch (isOccupied)
            {
                case true:
                    if (!OccupyingObject) { return; }
                    objectOccupyingTile = OccupyingObject;
                    SetTileType(TileType.OCCUPIEDTILE);
                    break;

                case false:
                    objectOccupyingTile = null;
                    SetTileType(TileType.TILE);
                    break;
            }
        }

        public void ResetTypeToDefault()
        {
            if (tileType == startingTileType) { return; }

            tileType = startingTileType;
            Destroy(tileObject);
            HandleTileSpawn();
        }

        public void ResetTileToType(TileType _originalType)
        {
            if (tileType == _originalType) { return; }

            tileType = _originalType;
            Destroy(tileObject);
            HandleTileSpawn();
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

        private void Update()
        {
            if (tileUI != null)
            {

                string tileID = $"{TileID.x}, {TileID.y}";

                tileUI.PopulateTileText(tileID);
            }
        }
    }
}
