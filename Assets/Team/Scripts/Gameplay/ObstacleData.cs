using UnityEngine;
using Team.Gameplay.GridSystem;

public class ObstacleData : MonoBehaviour
{
    [SerializeField]
    private TileID _myTileID;
    [SerializeField]
    private GridTile _myGridTile;

    public void MakeTileWalkable()
    {
        _myGridTile.SetTileType(TileType.TILE);
    }

    public void MakeTileUnwalkable()
    {
        _myGridTile.SetTileType(TileType.OCCUPIEDTILE);
    }

    public void UpdateObstacleTileData(TileID updatedTileID, GridTile updatedGridTile)
    {
        _myTileID = updatedTileID;
        _myGridTile = updatedGridTile;
    }

}
