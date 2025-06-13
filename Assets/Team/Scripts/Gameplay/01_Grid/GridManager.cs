using System.Collections.Generic;
using Team.Gameplay.GameLevelSystem;
using UnityEngine;

namespace Team.Gameplay.GridSystem
{
    [DefaultExecutionOrder(-10)]
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance = null;

        [SerializeField]
        private GameLevel gameLevel;

        public LevelTileCreator CurrentTileParent => gameLevel.LevelTiles;

        [SerializeField]
        private List<GridTile> tiles = new List<GridTile>(); //Dictionary is made from these tiles ***

        private Dictionary<TileID,GridTile> Grid = new Dictionary<TileID,GridTile>();

        //Obstacles data
        [Tooltip ("DONT FILL IT")]
        public List<GameObject> Obstacles = new List<GameObject>();
        

        private char[] gridCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

        [SerializeField]
        private GameObject ref_gridHolder;

        [SerializeField]
        private GameObject _defaultObstacle;
        public GameObject DefaultObstacle
        {
            get { return _defaultObstacle; }
        }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //Check if the tiles are empty
            if(gameLevel != null && tiles.Count == 0)
            {
                //Load all the tiles
                LoadTilesFromLevelTiles();
            }

            InitializeDictionary();
            FindAllObstacles();

        }

        #region Tile Creator Section

        public void SetCurrentLevelTile(GameLevel _level)
        {
            gameLevel = _level;

            //Load all the tiles
            LoadTilesFromLevelTiles();

            //Initialize Dictionary
            InitializeDictionary();
        }

        private void LoadTilesFromLevelTiles()
        {
            tiles.Clear();
            foreach (var tile in gameLevel.LevelTiles.Tiles)
            {
                tiles.Add(tile);
            }
        }

        #endregion

        #region Grid Dictionary Section

        private void FindAllObstacles()
        {
            foreach(var tile in tiles)
            {
                if(tile.tileType == TileType.OCCUPIEDTILE)
                {
                    Obstacles.Add(tile.ObjectOccupyingTile);
                }
            }
        }

        private void InitializeDictionary()
        {
            Grid.Clear();
            foreach(var item in tiles)
            {
                Grid.Add(item.TileID, item);
            }
        }
        //Helpers for grid creating
        public void RemoveTileFromGrid(TileID tileID,GridTile _tile)
        {
            //if (!tiles.Contains(_tile)) { Debug.LogWarning($"I cannot remove tile: {_tile.TileID}"); return; }
            //tiles.Remove(_tile);
            if (!Grid.ContainsKey(tileID)) return;
            Grid.Remove(tileID);
        }

        public void AddTileToGrid(TileID tileID, GridTile _tile)
        {
            //if (tiles.Contains(_tile)) return;
            //tiles.Add(_tile);

            if (Grid.ContainsKey(tileID)) return;
            Grid.Add(tileID, _tile);
        }

        /// <summary>
        /// Finds the tile associated with the given tile id
        /// </summary>
        /// <param name="tileID">Tile ID</param>
        /// <returns>Returns Grid tile with id</returns>
        public GridTile FindTile(TileID tileID)
        {
            if (!Grid.ContainsKey(tileID)) return null;

            return Grid[tileID];
        }

        [ContextMenu("Print Current Grid")]
        public void PrintGrid()
        {
            foreach (var tile in Grid)
            {
                Debug.Log($"Tile:(x ={tile.Key.x}, y= {tile.Key.y}) Value: {tile.Value.name}", tile.Value);
            }
        }
        #endregion
        
    }
}
