using Team.Gameplay.GridSystem;
using UnityEngine;

public class ObstacleData : MonoBehaviour, IDestroyable
{
    [SerializeField]
    private TileID _myTileID;

    [SerializeField]
    private TileID _startTileID;

    [SerializeField]
    private GridTile _startTile;

    public TileID CurrentTileID => _myTileID;
    [SerializeField]
    private GridTile _myGridTile;

    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _startTileID = _myTileID;
        _startTile = _myGridTile;
    }

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

    public void ResetToStart()
    {
        //Make my curent tile to tile walkable
        MakeTileWalkable();

        UpdateObstacleTileData(_startTileID, _startTile);

        Vector3 tilePosition = new Vector3(_startTile.transform.position.x, 1.5f, _startTile.transform.position.z);
        transform.position = tilePosition;

        //Make my start tile as unwalkable
        MakeTileUnwalkable();
    }
}
