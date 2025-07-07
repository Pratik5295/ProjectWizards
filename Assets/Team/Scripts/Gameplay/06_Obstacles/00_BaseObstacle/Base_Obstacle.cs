using System;
using UnityEngine;
using Team.Gameplay.GridSystem;
using Team.GameConstants;

public class Base_Obstacle : MonoBehaviour, IDestroyable
{
    [Header("Tile Variables")]
    [SerializeField]
    protected TileID _currentTileID;
    [SerializeField]
    protected TileID _previousTileID;

    [SerializeField]
    protected TileID _startTileID;

    [SerializeField]
    protected float ySpawnOffset = 1.5f;

    [SerializeField]
    protected GridTile _startTile;
    public TileID CurrentTileID => _currentTileID;
    [SerializeField]
    protected GridTile _currentGridTile;
    protected GridTile _previousGridTile;

    protected float OffsetValue;

    [SerializeField]
    protected Enum_GridDirection _startingDirection;


    [Header("Script References")]
    [SerializeField] protected GridManager ref_gridManager;

    [SerializeField] protected Base_Rotation baseRotation;
    public Base_Rotation BaseRotation
    {
        get { return baseRotation; }
    }

    [Header("Personal References")]
    [SerializeField]
    protected Collider _collider;
    [SerializeField]
    protected MeshRenderer _meshRenderer;

    [Header("Functionality Variables")]
    [SerializeField]
    protected bool canBeDestroyed = true;

    public bool CanBeDestroyed
    {
        get { return canBeDestroyed; }
    }

    private void Start()
    {
        _startTileID = _currentTileID;
        _startTile = _currentGridTile;

        if (gameObject.TryGetComponent<Base_Rotation>(out var ch))
        {
            _startingDirection = ch.DirectionFacing;
        }
        InitialiseObstacle(CurrentTileID, Enum_GridDirection.NORTH);
    }

    public virtual void InitialiseObstacle(TileID StartingTileID, Enum_GridDirection startingDirection)
    {
        ref_gridManager = GridManager.Instance;
        OffsetValue = MetaConstants.GridSlot_Offset;

        _currentTileID = StartingTileID;
        _previousTileID = _currentTileID;
        _startTileID = _currentTileID;

        _currentGridTile = ref_gridManager.FindTile(_currentTileID);
        _currentGridTile.SetObjectOccupyingTile(this.gameObject);

        _previousGridTile = _currentGridTile;

        baseRotation = GetComponent<Base_Rotation>();
        baseRotation.changeFacingDirection(startingDirection);
        Vector2 v2Dir = baseRotation.dirToV2(baseRotation.DirectionFacing);
        baseRotation.RotateToFaceDir(v2Dir);

        transform.position = new Vector3(_currentGridTile.TilePosition.x, _currentGridTile.TilePosition.y + ySpawnOffset, _currentGridTile.TilePosition.z);
        InitialiseObstacleData();
    }

    [ContextMenu("Initialise Obstacle Data")]
    public virtual void InitialiseObstacleData()
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

    public void UpdateCurrentTileID()
    {
        _currentTileID = _currentGridTile.TileID;
    }

    public void SetCurrentTile(TileID updatedTileID, GridTile updatedGridTile)
    {
        _currentTileID = updatedTileID;
        _currentGridTile = updatedGridTile;
    }

    public virtual void EnableObject()
    {
        if (!canBeDestroyed) { return; }
        _collider.enabled = true;
        _meshRenderer.enabled = true;
        _currentGridTile.SetObjectOccupyingTile(this.gameObject);
        MakeTileUnwalkable();
    }

    public virtual void DisableObject()
    {
        if (!canBeDestroyed) { return; }

        _collider.enabled = false;
        _meshRenderer.enabled = false;
        _currentGridTile.SetObjectOccupyingTile(null);
        MakeTileWalkable();
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

        if (gameObject.TryGetComponent<Base_Ch>(out var obstacle))
        {
            //obstacle.SetCurrentTile(_startTileID, _startTile);

            obstacle.InitialiseCharacter(_startTileID, _startingDirection);
        }
    }

    #region Tile data change functions
    public void MakeTileWalkable()
    {
        _currentGridTile.SetTileType(TileType.TILE);
    }

    public void MakeTileUnwalkable()
    {
        _currentGridTile.SetTileType(TileType.OCCUPIEDTILE);
    }

    public void UpdateObstacleTileData(TileID updatedTileID, GridTile updatedGridTile)
    {
        _currentTileID = updatedTileID;
        _currentGridTile = updatedGridTile;
    }
    #endregion
}
