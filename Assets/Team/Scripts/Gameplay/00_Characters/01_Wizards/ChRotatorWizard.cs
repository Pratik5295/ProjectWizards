using System.Collections;
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

    private GridTile[] _tilesToMove;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void UseAbility()
    {
        Vector2 dirOffset = baseRotation.GetFacingDirection() * _abilityStartOffset;
        Vector2 dirOffsetAndTileID = new Vector2(currentTileID.x + dirOffset.x, currentTileID.y + dirOffset.y);

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

        _tilesToMove = new GridTile[5];
        _tilesToMove[0] = centerTile;
        _tilesToMove[1] = forwardTile;
        _tilesToMove[2] = backwardTile;
        _tilesToMove[3] = rightTile;
        _tilesToMove[4] = leftTile;

        for (int i = 1; i < _tilesToMove.Length; i++)
        {
            if (!_tilesToMove[i]) { return; }
        }

        _rotatorHolder = new GameObject("_rotatorHolder");
        _rotatorHolder.transform.position = centerTile.TilePosition;
        _rotatorHolder.transform.SetParent(ref_gridManager.transform.GetChild(0));

        for (int i = 0; i < _tilesToMove.Length; i++)
        {
            _tilesToMove[i].transform.SetParent(_rotatorHolder.transform);
            _tilesToMove[i].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.darkSlateGray;
        }

        StartCoroutine(LerpUpDown(true));
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
