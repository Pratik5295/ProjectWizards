using System.Collections.Generic;
using Team.GameConstants;
using UnityEngine;

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
        private GameObject _defaultObstacle;
        public GameObject DefaultObstacle
        {
            get { return _defaultObstacle; }
        }

        [SerializeField]
        private GameObject _defaultTile;
       

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

    }
}
