using System.Collections.Generic;
using System.Linq;
using Team.GameConstants;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public static readonly char[] gridCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

        public const float GridSlot_Offset = 0.18f;

        public const float GridY = 0.5f;

        public const float TileSize = 1.3f;

        public static string GetNewName(int x, int y)
        {
            return $"Tile: {gridCharArray[x]} {x}, {y}";
        }
    }
}

namespace Team.Gameplay.GridSystem
{
    /// <summary>
    /// Responsible for only creating the grid tiles and creating a prefab out of it.
    /// Tiles would be created as a child of this gameobject
    /// </summary>
    public class LevelTileCreator : MonoBehaviour
    {

        [SerializeField] private Vector2 Max_GridSize;
        public Vector2 Max_GridSize_Acc
        {
            get { return Max_GridSize; }
        }


        [SerializeField]
        private List<GridTile> tiles = new List<GridTile>();

        public List<GridTile> Tiles => tiles;

        [SerializeField]
        private List<OilTile> oilTiles = new List<OilTile>();
        public List<OilTile> OilTiles => oilTiles;


        [SerializeField]
        private GameObject _defaultObstacle;
        public GameObject DefaultObstacle
        {
            get { return _defaultObstacle; }
        }


        [SerializeField]
        private GameObject _defaultTile;

        [SerializeField]
        private GameObject _oilTile;

        [ContextMenu("Clear Grid")]
        public void ClearGrid()
        {
            foreach (var tile in tiles)
            {
                DestroyImmediate(tile.gameObject); 
            }

            tiles.Clear();

        }


        //Creates Grid and fills up grid with references to tiles and locations.
        [ContextMenu("Create Grid")]
        public void CreateGrid()
        {
            if (tiles.Count > 0 || transform.childCount > 0)
            {
                Debug.LogError("Grid already exists, destroy the old grid first. No new grid created");
                return;
            }

            //ref_gridHolder.transform.SetParent(transform);

            for (int x = 0; x < Max_GridSize.x; x++)
            {
                for (int y = 0; y < Max_GridSize.y; y++)
                {
                    float positionX = x * (MetaConstants.TileSize + MetaConstants.GridSlot_Offset);
                    float positionY = y * (MetaConstants.TileSize + MetaConstants.GridSlot_Offset);

                    //Find the random tile
                    var tileToSpawn = _defaultTile;

                    var spawnedTile = SpawnTile(tileToSpawn, positionX, MetaConstants.GridY, positionY);
                    var gridTile = spawnedTile.GetComponent<GridTile>();
                    TileID tileID = new TileID(x, y);
                    bool isWalkable = gridTile.Init(this,tileID); //Update this to look cleaner and error check

                    spawnedTile.name = $"Tile: {MetaConstants.gridCharArray[x]} {x}, {y}";

                    if (isWalkable)
                    {

                        //Grid.Add(tileID,gridTile);
                        tiles.Add(gridTile);
                    }
                }
            }
        }

        private GameObject SpawnTile(GameObject _tile, float x, float y, float z)
        {
            return Instantiate(_tile, new Vector3(x, y, z), Quaternion.identity, transform);
        }


        #region Creating and removing tiles
        public void CreateNewOilTile(TileID previousTileID, float positionX, float positionZ)
        {
            //Remove old tile from list
            var tile = GetTile(previousTileID);
            RemoveTile(tile.GetComponent<GridTile>());
            DestroyImmediate(tile);

            var spawnedTile = SpawnTile(_oilTile, positionX, MetaConstants.GridY, positionZ);
            var gridTile = spawnedTile.GetComponent<OilTile>();
            TileID tileID = new TileID(previousTileID.x, previousTileID.y);
            gridTile.Init(this, tileID, true); //Update this to look cleaner and error check

            spawnedTile.name = $"Tile: {MetaConstants.gridCharArray[previousTileID.x]} {previousTileID.x}, {previousTileID.y}";
            tiles.Add(gridTile);
            oilTiles.Add(gridTile);
            DirtySaveTileChanges();
        }

        public void RemoveTile(GridTile currentTile)
        {
            tiles.Remove(currentTile);
            if (currentTile.gameObject.GetComponent<OilTile>())
            {
                oilTiles.Remove(currentTile.gameObject.GetComponent<OilTile>());
            }
        }

        public GridTile CreateNewTile(GridTile currentTile)
        {
            float positionX = currentTile.TileID.x;
            float positionY = currentTile.TileID.y;

            var spawnedTile = SpawnTile(_defaultTile, positionX, MetaConstants.GridY, positionY);
            var gridTile = spawnedTile.GetComponent<GridTile>();
            TileID tileID = currentTile.TileID;
            bool isWalkable = gridTile.Init(this, tileID); //Update this to look cleaner and error check

            spawnedTile.name = $"Tile: {MetaConstants.gridCharArray[tileID.x]} {tileID.x}, {tileID.y}";
            tiles.Add(gridTile);
            return gridTile;
        }
        #endregion
        public void DirtySaveTileChanges()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }

        public void SetDefaultTile(GameObject _tile)
        {
            _defaultTile = _tile;
        }

        public void SetDefaultObstacle(GameObject _obstacle)
        {
            _defaultObstacle = _obstacle;
        }

        public void SetGridSize(Vector2 _size)
        {
            Max_GridSize = _size;
        }

        public GameObject GetTile(TileID _tileID)
        {
            var tile = tiles.Single(tile => tile.TileID == _tileID);

            return tile.gameObject; 
        }

        [ContextMenu("Show Tile UI")]
        public void ShowTileUI()
        {
            foreach(var tile in tiles)
            {
                tile.ShowTileUI();
            }
        }

        [ContextMenu("Hide Tile UI")]
        public void HideTileUI()
        {
            foreach(var tile in tiles)
            {
                tile.HideTileUI();
            }
        }
    }
}
