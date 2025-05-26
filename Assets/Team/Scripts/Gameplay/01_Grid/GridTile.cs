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
        public TileID TileID; // ID of the tile in the grid

        public Vector3 TilePosition => transform.position;

        public GameObject tilePrefab;

        public TileType tileType;

        public TileDirection Direction; //Rotation 

        [SerializeField]
        private GridManager gridManager;

        [SerializeField]
        private GameObject tileObject = null; //The created tile object

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
            gridManager?.RemoveTileFromGrid(TileID,this);
        }

        [ContextMenu("Set Tile to Object")]
        public void SetTileObject()
        {
            tileType = TileType.TILE;
            tileObject = SpawnTileObject();
            gridManager?.AddTileToGrid(TileID, this);
        }
    }
}
