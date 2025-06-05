using UnityEngine;
using Team.Gameplay.GridSystem;

public class ObstacleData : MonoBehaviour
{
    private TileID _myTileID;
    private GridTile _myGridTile;


    public void UpdateObstacleTileData(TileID updatedTileID, GridTile updatedGridTile)
    {
        _myTileID = updatedTileID;
        _myGridTile = updatedGridTile;
    }

}
