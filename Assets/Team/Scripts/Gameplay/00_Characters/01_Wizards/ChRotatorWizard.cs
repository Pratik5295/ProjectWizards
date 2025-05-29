using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.Gameplay.GridSystem;
using UnityEngine;

public class ChRotatorWizard : Base_Ch
{

    [Header("Rotator Wizard Variables")]
    [SerializeField]
    private int _abilityStartOffset;

    [SerializeField]
    private float _lerpDuration = 0.25f;
    [SerializeField]
    private float _lerpDelay = 0.01f;

    private GameObject _rotatorHolder;

    private List<GridTile> _tilesToMove;

    public override void UseAbility()
    {
        _tilesToMove = new List<GridTile>();
        Vector2 dirOffset = baseRotation.GetFacingDirection() * _abilityStartOffset;
        Vector2 dirOffsetAndTileID = new Vector2(_currentTileID.x + dirOffset.x, _currentTileID.y + dirOffset.y);

        GridTile centerTile = ref_gridManager.FindTile(new TileID((int)dirOffsetAndTileID.x, (int)dirOffsetAndTileID.y));
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

        for (int i = 1; i < _tilesToMove.Count; i++)
        {
            if (!_tilesToMove[i]) { return; }
        }

        _rotatorHolder = new GameObject("_rotatorHolder");
        _rotatorHolder.transform.position = centerTile.TilePosition;
        _rotatorHolder.transform.SetParent(ref_gridManager.transform.GetChild(0));

        for (int i = 0; i < _tilesToMove.Count; i++)
        {
            if (_tilesToMove[i].ObjectOccupyingTile)
            {
                _tilesToMove[i].ParentUnparentOccupyingObject();
            }
            _tilesToMove[i].transform.SetParent(_rotatorHolder.transform);
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.darkSlateGray;
        }
        TileDataChanges();
        StartCoroutine(LerpUpDown(true));
    }

    private void TileDataChanges()
    {

        for(int i = 1; i < _tilesToMove.Count; i++)
        {
            //Remove Tiles from dictionary.
            ref_gridManager.RemoveTileFromGrid(_tilesToMove[i].TileID, _tilesToMove[i]);

            switch (i) // Change Tile ID and rename to new tile name.
            {
                case 1:
                    _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x + 1, _tilesToMove[i].TileID.y - 1);
                    _tilesToMove[i].name = ref_gridManager.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                    break;
                case 2:
                    _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x - 1, _tilesToMove[i].TileID.y + 1);
                    _tilesToMove[i].name = ref_gridManager.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                    break;
                case 3:
                    _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x - 1, _tilesToMove[i].TileID.y - 1);
                    _tilesToMove[i].name = ref_gridManager.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
                    break;
                case 4:
                    _tilesToMove[i].TileID = new TileID(_tilesToMove[i].TileID.x + 1, _tilesToMove[i].TileID.y + 1);
                    _tilesToMove[i].name = ref_gridManager.GetNewName(_tilesToMove[i].TileID.x, _tilesToMove[i].TileID.y);
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
            _tilesToMove[i].transform.SetParent(ref_gridManager.transform.GetChild(0));
            if (_tilesToMove[i].ObjectOccupyingTile)
            {
                _tilesToMove[i].ObjectOccupyingTile.GetComponent<Base_Ch>().UpdateCurrentTileID();
                _tilesToMove[i].ParentUnparentOccupyingObject();
            }
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        //_tilesToMove.Clear();
    }

    private IEnumerator LerpUpDown(bool isLerpingUp)
    {
        float elapsedTime = 0f;
        Vector3 _holderStartPos = _rotatorHolder.transform.position;
        Vector3 _holderEndPos = new Vector3(_rotatorHolder.transform.position.x, 0.5f, _rotatorHolder.transform.position.z); //Need Pratik to add a default position to grid tile script, so that the hard coded value can be changed.

        if (isLerpingUp)
        {
            _holderEndPos = new Vector3(_rotatorHolder.transform.position.x, _tilesToMove[0].TilePosition.y * 3, _rotatorHolder.transform.position.z);
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

        Quaternion startingRotation = _rotatorHolder.transform.rotation;

        Vector3 targetV3 = new Vector3(_rotatorHolder.transform.rotation.x, _rotatorHolder.transform.rotation.y + 90, _rotatorHolder.transform.rotation.z);
        Quaternion targetRotation = Quaternion.Euler(_rotatorHolder.transform.rotation.x, _rotatorHolder.transform.rotation.y + 90, _rotatorHolder.transform.rotation.z);

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
}
