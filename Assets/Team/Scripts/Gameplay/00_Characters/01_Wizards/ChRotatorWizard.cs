using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.Gameplay.GridSystem;
using UnityEngine;
using Team.GameConstants;
using UnityEditor.Experimental.GraphView;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public enum Enum_Rotation
        {
            Clockwise,
            AntiClockwise
        }

        public static float lerpUpAmount = 3f;

        public static float holderLerpUpOffset = 0.5f;
    }
}

public class ChRotatorWizard : Base_Ch
{

    [Header("Rotator Wizard Variables")]
    private MetaConstants.Enum_Rotation rotation = MetaConstants.Enum_Rotation.Clockwise;
    [SerializeField]
    private int _abilityStartOffset;

    [SerializeField]
    private float _lerpDuration = 0.25f;
    [SerializeField]
    private float _lerpDelay = 0.01f;

    private GameObject _rotatorHolder;

    private GridTile centerTile;
    private List<GridTile> _tilesToMove;

    public override void UseAbility()
    {
        GetTilesToRotate();
        if (!centerTile)
        {
            Debug.Log("Cant Execute Ability as no tiles no center tile.");
            OnTurnComplete();
            return;
        }

        for (int i = 1; i < _tilesToMove.Count; i++)
        {
            if (!_tilesToMove[i]) { return; }
        }

        _rotatorHolder = new GameObject("_rotatorHolder");
        _rotatorHolder.transform.position = centerTile.TilePosition;
        _rotatorHolder.transform.SetParent(ref_gridManager.CurrentTileParent.transform);

        for (int i = 0; i < _tilesToMove.Count; i++)
        {
            if (_tilesToMove[i].ObjectOccupyingTile)
            {
                _tilesToMove[i].ParentOccupyingObject();
            }
            _tilesToMove[i].transform.SetParent(_rotatorHolder.transform);
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.darkSlateGray;
        }
        rotation = MetaConstants.Enum_Rotation.Clockwise;

        PlayerMove move = new PlayerMove(false);
        HistoryStack.Push(move);

        TileDataChanges();
        StartCoroutine(LerpUpDown(true));
    }

    [ContextMenu("Undo Rotation")]
    public override void UndoAction()
    {
        Debug.Log($"Moves count: {HistoryStack.Count}");

        while (HistoryStack.Count > 0)
        {
            var move = HistoryStack.Pop();

            if (move.wasMoved)
            {
                UndoMovement();
            }
            else
            {
                UndoRotate();
            }
        }

        OnTurnComplete?.Invoke();
    }

    private void UndoRotate()
    {
        GetTilesToRotate();

        for (int i = 1; i < _tilesToMove.Count; i++)
        {
            if (!_tilesToMove[i]) { return; }
        }

        _rotatorHolder = new GameObject("_rotatorHolder");
        _rotatorHolder.transform.position = centerTile.TilePosition;
        _rotatorHolder.transform.SetParent(ref_gridManager.CurrentTileParent.transform);

        for (int i = 0; i < _tilesToMove.Count; i++)
        {
            if (_tilesToMove[i].ObjectOccupyingTile)
            {
                _tilesToMove[i].ParentOccupyingObject();
            }
            _tilesToMove[i].transform.SetParent(_rotatorHolder.transform);
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.darkSlateGray;
        }
        rotation = MetaConstants.Enum_Rotation.AntiClockwise;
        TileDataChanges();
        StartCoroutine(LerpUpDown(true));
    }

    private void GetTilesToRotate()
    {
        _tilesToMove = new List<GridTile>();
        Vector2 dirOffset = baseRotation.GetFacingDirection() * _abilityStartOffset;
        Vector2 dirOffsetAndTileID = new Vector2(_currentTileID.x + dirOffset.x, _currentTileID.y + dirOffset.y);

        centerTile = ref_gridManager.FindTile(new TileID((int)dirOffsetAndTileID.x, (int)dirOffsetAndTileID.y));

        if (!centerTile)
        {
            Debug.Log("Cant Execute Ability as no tiles no center tile.");
            return;
        }


        GridTile forwardTile = ref_gridManager.FindTile(new TileID(centerTile.TileID.x, centerTile.TileID.y + 1));
        GridTile backwardTile = ref_gridManager.FindTile(new TileID(centerTile.TileID.x, centerTile.TileID.y - 1));
        GridTile rightTile = ref_gridManager.FindTile(new TileID(centerTile.TileID.x + 1, centerTile.TileID.y));
        GridTile leftTile = ref_gridManager.FindTile(new TileID(centerTile.TileID.x - 1, centerTile.TileID.y));

        _tilesToMove.Add(centerTile);
        _tilesToMove.Add(forwardTile);
        _tilesToMove.Add(backwardTile);
        _tilesToMove.Add(rightTile);
        _tilesToMove.Add(leftTile);
    }

    private void TileDataChanges()
    {

        for(int i = 1; i < _tilesToMove.Count; i++)
        {
            //Remove Tiles from dictionary.
            ref_gridManager.RemoveTileFromGrid(_tilesToMove[i].TileID, _tilesToMove[i]);
            GameObject characterOnTile = null;
            if (_tilesToMove[i].ObjectOccupyingTile && _tilesToMove[i].ObjectOccupyingTile.CompareTag("Character"))
            {
                characterOnTile = _tilesToMove[i].ObjectOccupyingTile;
            }
            if (characterOnTile)
            {
                Base_Rotation charactersRotationSc = characterOnTile.GetComponent<Base_Rotation>();
                if(rotation == MetaConstants.Enum_Rotation.AntiClockwise) 
                { 
                    charactersRotationSc.changeFacingDirection(DirectionUtilities.RotateAntiClockwise(charactersRotationSc.DirectionFacing));
                }
                if (rotation == MetaConstants.Enum_Rotation.Clockwise)
                {
                    charactersRotationSc.changeFacingDirection(DirectionUtilities.RotateClockwise(charactersRotationSc.DirectionFacing));
                }
            }
            switch (rotation)
            {
                case MetaConstants.Enum_Rotation.Clockwise:
                    switch (i) // Change Tile ID and rename to new tile name.
                    {
                        case 1:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x + 1, _tilesToMove[i].TileID.y - 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                        case 2:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x - 1, _tilesToMove[i].TileID.y + 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                        case 3:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x - 1, _tilesToMove[i].TileID.y - 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                        case 4:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x + 1, _tilesToMove[i].TileID.y + 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                    }
                  break;
                case MetaConstants.Enum_Rotation.AntiClockwise:
                    switch (i) // Change Tile ID and rename to new tile name.
                    {
                        case 1:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x - 1, _tilesToMove[i].TileID.y - 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                        case 2:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x + 1, _tilesToMove[i].TileID.y + 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                        case 3:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x - 1, _tilesToMove[i].TileID.y + 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                        case 4:
                            _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x + 1, _tilesToMove[i].TileID.y - 1);
                            _tilesToMove[i].name = MetaConstants.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                            break;
                    }
                    break;
            }


        }

        for(int j = 1; j < _tilesToMove.Count; j++) //Re-add tile To dictionary, after frame has removed from dictionary.
        {
            ref_gridManager.AddTileToGrid(_tilesToMove[j].TileID, _tilesToMove[j]);
        }
    }

    private void CleanUpTiles()
    {
        _rotatorHolder.transform.DetachChildren();
        Destroy(_rotatorHolder);

        for(int i = 0; i < _tilesToMove.Count; i++)
        {
            _tilesToMove[i].transform.SetParent(ref_gridManager.CurrentTileParent.transform);
            if (_tilesToMove[i].ObjectOccupyingTile)
            {
                if (_tilesToMove[i].ObjectOccupyingTile.CompareTag(MetaConstants.CharacterTag))
                {
                    _tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Ch>().UpdateCurrentTileID();
                }
                else if (_tilesToMove[i].ObjectOccupyingTile.CompareTag(MetaConstants.EnvironmentTag))
                {
                    _tilesToMove[i].ObjectOccupyingTile.GetComponent<ObstacleData>().UpdateObstacleTileData(_tilesToMove[i].TileID, _tilesToMove[i]);
                    if (_tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Ch>())
                    {
                        _tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Ch>().UpdateCurrentTileID();
                    }
                }

                _tilesToMove[i].UnparentOccupyingObject();
            }
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        OnTurnComplete?.Invoke();
        //_tilesToMove.Clear();
    }

    private IEnumerator LerpUpDown(bool isLerpingUp)
    {
        float elapsedTime = 0f;
        Vector3 _holderStartPos = _rotatorHolder.transform.position;
        Vector3 _holderEndPos = new Vector3(_rotatorHolder.transform.position.x, MetaConstants.holderLerpUpOffset, _rotatorHolder.transform.position.z); //Need Pratik to add a default position to grid tile script, so that the hard coded value can be changed.

        if (isLerpingUp)
        {
            _holderEndPos = new Vector3(_rotatorHolder.transform.position.x, _tilesToMove[0].TilePosition.y * MetaConstants.lerpUpAmount, _rotatorHolder.transform.position.z);
        }

        while (elapsedTime < _lerpDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = elapsedTime / _lerpDuration;

            _rotatorHolder.transform.position = Vector3.Lerp(_holderStartPos, _holderEndPos, lerpAmount);

            yield return null;
        }
        if (isLerpingUp) { StartCoroutine(RotateLerp()); }
        if (!isLerpingUp) { CleanUpTiles(); }
    }


    private IEnumerator RotateLerp()
    {
        float elapsedTime = 0f;
        float RotationValue = GetRotationValue(rotation);

        Quaternion startingRotation = _rotatorHolder.transform.rotation;

        Vector3 targetV3 = new Vector3(_rotatorHolder.transform.rotation.x, _rotatorHolder.transform.rotation.y + RotationValue, _rotatorHolder.transform.rotation.z);
        Quaternion targetRotation = Quaternion.Euler(_rotatorHolder.transform.rotation.x, _rotatorHolder.transform.rotation.y + RotationValue, _rotatorHolder.transform.rotation.z);

        while (elapsedTime < _lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float fraction = elapsedTime / _lerpDuration;

            _rotatorHolder.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, fraction);

            yield return null;
        }

        _rotatorHolder.transform.rotation = targetRotation;
        StartCoroutine(LerpUpDown(false));
    }

    private float GetRotationValue(MetaConstants.Enum_Rotation Rotation)
    {
        switch (Rotation)
        {
            case MetaConstants.Enum_Rotation.Clockwise:
                return 90f;
            case MetaConstants.Enum_Rotation.AntiClockwise:
                return -90f;
        }
        Debug.LogWarning($"{gameObject}: Get Rotation Value: Wasnt able to be determined whether it was clockwise or Anti-Clockwise.");
        return 90f;
    }
}
