using UnityEngine;
using Team.Gameplay.GridSystem;

public class ObstacleData : MonoBehaviour, IDestroyable
{
    [SerializeField]
    private TileID _myTileID;

    public TileID CurrentTileID => _myTileID;
    [SerializeField]
    private GridTile _myGridTile;

    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private MeshRenderer _meshRenderer;

    [ContextMenu("Initialise Obstacle Data")]
    public void InitialiseObstacleData()
    {
        if (GetComponent<Collider>())
        {
            _collider = GetComponent<Collider>();
        }
        if (GetComponent<MeshRenderer>())
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        if (!_collider)
        {
            _collider = GetComponentInChildren<Collider>();
        }
        if (!_meshRenderer)
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
    }


    public void EnableObject()
    {
        _collider.enabled = true;
        _meshRenderer.enabled = true;
        MakeTileWalkable();
    }

    public void DisableObject()
    {
        _collider.enabled = false;
        _meshRenderer.enabled = false;
        MakeTileUnwalkable();
    }

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
