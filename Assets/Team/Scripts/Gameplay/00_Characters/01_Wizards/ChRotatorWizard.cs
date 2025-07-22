using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

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

    [Header("Rotation VFX")]
    [SerializeField] private GameObject _rotationLandingVFX;
    private float _landingVFXOffset;
    private VFXManager _landingVFXManager;

    private Coroutine activeCoroutine = null;

    private void OnDestroy()
    {
        if (_landingVFXManager != null)
        {
            Destroy(_landingVFXManager);
        }
    }

    public override void InitialiseCharacter(TileID StartingTileID, Enum_GridDirection startingDirection)
    {
        base.InitialiseCharacter(StartingTileID, startingDirection);

        _landingVFXManager = Instantiate(_rotationLandingVFX).GetComponent<VFXManager>();
        _landingVFXOffset = _landingVFXManager.transform.position.y;
        _landingVFXManager.transform.position = new Vector3(transform.position.x, _landingVFXOffset, transform.position.z);
    }

    public async override Task UseAbility()
    {
        abilityWaiter = new TaskCompletionSource<bool>();
        OnCastBark();

        GetTilesToRotate();

        if (!centerTile || _tilesToMove.Count != 5)
        {
            Debug.Log("Cant Execute Ability as no tiles no center tile.");
            OnAbilityCompleted();
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
            GameTurnManager.Instance.AddRotatedTile(_tilesToMove[i]);
            if (_tilesToMove[i].ObjectOccupyingTile)
            {
                _tilesToMove[i].ParentOccupyingObject();
            }
            _tilesToMove[i].transform.SetParent(_rotatorHolder.transform);
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.darkSlateGray;
        }
        rotation = MetaConstants.Enum_Rotation.Clockwise;

        PlayerMove move = new PlayerMove(CurrentTileID, PlayerMoveType.ROTATED, baseRotation.DirectionFacing);
        HistoryStack.Push(move);

        TileDataChanges();
        activeCoroutine = StartCoroutine(LerpUpDown(true));

        await abilityWaiter.Task;
    }

    [ContextMenu("Undo Rotation")]
    protected override async Task CharacterUndoStack()
    {
        undoAwaiter = new TaskCompletionSource<bool>();

        Debug.Log($"{gameObject.name} Moves count: {HistoryStack.Count}");

        if (HistoryStack.Count == 0)
        {
        }
        else
        {
            while (HistoryStack.Count > 0)
            {
                var move = HistoryStack.Pop();

                //Debug.Log($"{gameObject.name} Move was: {move.wasMoved}");
                
                if (move.Type == PlayerMoveType.PUSH)
                {
                    Debug.Log("Pratik why is rotator handling push?");
                    move.interactedWith.UndoMovement(this, move.movedFrom);
                }
                else if(move.Type == PlayerMoveType.ROTATED)
                {
                    await UndoRotate();
                }
            }
            await undoAwaiter.Task;
        }
    }

    private async Task UndoRotate()
    {
        GetTilesToRotate();

        if (!centerTile || _tilesToMove.Count != 5)
        {
            Debug.Log("Cant Execute Ability as no tiles no center tile.");
           // OnTurnComplete?.Invoke();
            return;
        }

        undoAbilityWaiter = new TaskCompletionSource<bool>();
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
        activeCoroutine = StartCoroutine(LerpUpDown(true,true));

        await undoAbilityWaiter.Task;
    }

    private void GetTilesToRotate()
    {
        _tilesToMove = new List<GridTile>();
        Vector2 dirOffset = baseRotation.GetFacingDirection() * _abilityStartOffset;
        Vector2 dirOffsetAndTileID = new Vector2(_currentTileID.x + dirOffset.x, _currentTileID.y + dirOffset.y);

        centerTile = ref_gridManager.FindTile(new TileID((int)dirOffsetAndTileID.x, (int)dirOffsetAndTileID.y));

        _landingVFXManager.transform.SetParent(centerTile.transform);
        _landingVFXManager.transform.localPosition = new Vector3(0, _landingVFXOffset, 0);

        if (!centerTile)
        {
            Debug.Log("Cant Execute Ability as no tiles no center tile.");
            return;
        }

        GridTile[] NeighbourTiles = centerTile.FindNeighbouringTiles();

        _tilesToMove.Add(centerTile);

        for (int i = 1; i < NeighbourTiles.Length; i++)
        {
            if (NeighbourTiles[i] != null)
            {
                _tilesToMove.Add(NeighbourTiles[i]);
            }
        }
    }

    private void TileDataChanges()
    {

        for (int i = 0; i < _tilesToMove.Count; i++)
        {
            //Remove Tiles from dictionary.
            if (i != 0) // Only execute the following code if i is not 0.
            {
                ref_gridManager.RemoveTileFromGrid(_tilesToMove[i].TileID, _tilesToMove[i]);
            }

            GameObject characterOnTile = null;
            if (_tilesToMove[i].ObjectOccupyingTile && _tilesToMove[i].ObjectOccupyingTile.CompareTag("Character"))
            {
                characterOnTile = _tilesToMove[i].ObjectOccupyingTile;
            }
            if (characterOnTile)
            {
                Base_Rotation charactersRotationSc = characterOnTile.GetComponent<Base_Rotation>();
                if (rotation == MetaConstants.Enum_Rotation.AntiClockwise)
                {
                    charactersRotationSc.changeFacingDirection(DirectionUtilities.RotateAntiClockwise(charactersRotationSc.DirectionFacing));
                }
                if (rotation == MetaConstants.Enum_Rotation.Clockwise)
                {
                    charactersRotationSc.changeFacingDirection(DirectionUtilities.RotateClockwise(charactersRotationSc.DirectionFacing));
                }
            }

            if (i == 0) //Skip to next iteration in the loop if its the center tile.
            {
                continue;
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

        for (int j = 1; j < _tilesToMove.Count; j++) //Re-add tile To dictionary, after frame has removed from dictionary.
        {
            ref_gridManager.AddTileToGrid(_tilesToMove[j].TileID, _tilesToMove[j]);
        }
    }

    private void CleanUpTiles(bool _isUndoing)
    {
        _rotatorHolder.transform.DetachChildren();
        Destroy(_rotatorHolder);

        for (int i = 0; i < _tilesToMove.Count; i++)
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
                    _tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Obstacle>().UpdateObstacleTileData(_tilesToMove[i].TileID, _tilesToMove[i]);
                    if (_tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Obstacle>())
                    {
                        _tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Obstacle>().UpdateCurrentTileID();
                    }
                }

                _tilesToMove[i].UnparentOccupyingObject();
            }
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = _tilesToMove[i].normalColour;
        }
        //_tilesToMove.Clear();

        activeCoroutine = null;

        if (!_isUndoing)
        {
            OnAbilityCompleted();
        }
        else
        {
            Debug.Log("Pratik firing undo rotate here");
            OnUndoAbilityCompleted();
        }
    }

    private IEnumerator LerpUpDown(bool isLerpingUp, bool _isUndoing = false)
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
        if (isLerpingUp) 
        {
            if (!_isUndoing)
            {
                activeCoroutine = StartCoroutine(RotateLerp());
            }
            else
            {
                activeCoroutine = StartCoroutine(RotateLerp(true));
            }
        }
        if (!isLerpingUp)
        {
            /*GameObject instance = Instantiate(_rotationLandingVFX, centerTile.transform);
            instance.transform.localPosition = new Vector3(0, instance.transform.position.y, 0);*/
            _landingVFXManager.transform.localPosition = new Vector3(0, _landingVFXOffset, 0);
            _landingVFXManager.EnableParticleEffectChildren();

            CleanUpTiles(_isUndoing);

           
        }
    }


    private IEnumerator RotateLerp(bool _isUndoing = false)
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
        if (!_isUndoing)
        {
            activeCoroutine = StartCoroutine(LerpUpDown(false));
        }
        else
        {
            activeCoroutine = StartCoroutine(LerpUpDown(false, true));
        }
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
