using UnityEngine;

namespace Team.Gameplay.GridSystem
{
    public enum TileType
    {
        EMPTY = 0, //Highlights no tile
        TILE = 1 //Tile has content (not an empty tile)
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
        public Vector2 GridID; // Position . TODO: Update it to highlight position properly in vector?

        public GameObject tilePrefab;

        public TileType tileType;

        public TileDirection Direction; //Rotation 

        [SerializeField]
        private Sc_GridManager gridManager;

        [SerializeField]
        private GameObject tileObject = null; //The created tile object

        /// <summary>
        /// Initialize the tile
        /// </summary>
        public bool Init(Sc_GridManager _gridManager)
        {
            gridManager = _gridManager;

            //Check if spawn tile
            if (ShouldSpawnTile())
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

        private bool ShouldSpawnTile()
        {
            return tileType == TileType.TILE;
        }

        [ContextMenu("Set Tile Empty")]
        public void SetTileEmpty()
        {
            tileType = TileType.EMPTY;
            DestroyImmediate(tileObject);
            tileObject = null;
            gridManager?.RemoveTileFromGrid(this);
        }

        [ContextMenu("Set Tile to Object")]
        public void SetTileObject()
        {
            tileType = TileType.TILE;
            SpawnTileObject();
            gridManager?.AddTileToGrid(this);
        }
    }
}
