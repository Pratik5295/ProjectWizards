using System.Collections.Generic;
using UnityEngine;

namespace Team.Gameplay.GridSystem
{

    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Vector2 Max_GridSize;
        [SerializeField] private float gridY = 0.5f;   //Where to spawn the grid wrt height, Default is 0.5f
        [SerializeField] private float tileSize = 2f; //Width and height of the tile assuming each tile is square
        public Vector2 Max_GridSize_Acc
        {
            get { return Max_GridSize; }
        }
        [SerializeField] private float GridSlot_Offset;

        [SerializeField] private GameObject HolderPrefab;

        [SerializeField]
        private List<GameObject> TileMap = new List<GameObject>();

        [SerializeField]
        private Dictionary<TileID,GridTile> Grid = new Dictionary<TileID,GridTile>();
        

        private char[] gridCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

        [SerializeField]
        private GameObject ref_gridHolder;

        [ContextMenu("Print Current Grid")]
        public void PrintGrid()
        {
            foreach(var tile in Grid)
            {
                Debug.Log($"Tile:(x ={tile.Key.x}, y= {tile.Key.y}) Value: {tile.Value.name}",tile.Value);
            }
        }

        [ContextMenu("Clear Grid")]
        public void ClearGrid()
        {
            Grid.Clear();

            if (ref_gridHolder == null)
            {
                if (transform.childCount > 0)
                {
                    ref_gridHolder = transform.GetChild(0).gameObject;
                }
            }

            DestroyImmediate(ref_gridHolder);

            ref_gridHolder = null; //Just to avoid the "missing" feedback in inspector

        }

        //Creates Grid and fills up grid with references to tiles and locations.
        [ContextMenu("Create Grid")]
        public void CreateGrid()
        {
            if (Grid.Count > 0 || ref_gridHolder != null || transform.childCount > 0)
            {
                Debug.LogError("Grid already exists, destroy the old grid first. No new grid created");
                return;
            }

            ref_gridHolder = Instantiate(HolderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ref_gridHolder.transform.SetParent(transform);

            for (int x = 0; x < Max_GridSize.x; x++)
            {
                for (int y = 0; y < Max_GridSize.y; y++)
                {
                    float positionX = x * (tileSize + GridSlot_Offset);
                    float positionY = y * (tileSize + GridSlot_Offset);

                    //Find the random tile
                    var tileToSpawn = GetRandomTile();

                    var spawnedTile = SpawnTile(tileToSpawn,positionX, gridY, positionY);
                    var gridTile = spawnedTile.GetComponent<GridTile>();
                    TileID tileID = new TileID(x, y);
                    bool isWalkable = gridTile.Init(this, tileID); //Update this to look cleaner and error check

                    spawnedTile.name = $"Tile: {gridCharArray[x]} {x}, {y}";

                    if (isWalkable)
                    {
                        
                        Grid.Add(tileID,gridTile);
                    }
                }
            }
        }

        private GameObject SpawnTile(GameObject _tile,float x, float y, float z)
        {
            return Instantiate(_tile, new Vector3(x, y, z), Quaternion.identity, ref_gridHolder.transform);
        }

        /// <summary>
        /// Returns a random GameObject from the gameTiles list.
        /// </summary>
        public GameObject GetRandomTile()
        {
            if (TileMap == null || TileMap.Count == 0)
            {
                Debug.LogWarning("Game tiles list is empty or not assigned.");
                return null;
            }

            int randomIndex = Random.Range(0, TileMap.Count);
            return TileMap[randomIndex];
        }


        //Helpers for grid creating
        public void RemoveTileFromGrid(TileID tileID,GridTile _tile)
        {
            if (!Grid.ContainsKey(tileID)) return;
            Grid.Remove(tileID);
        }

        public void AddTileToGrid(TileID tileID, GridTile _tile)
        {
            if (Grid.ContainsKey(tileID)) return;
            Grid.Add(tileID, _tile);
        }
    }
}
