using Team.Enum.Character;
using Team.GameConstants;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

public class OilTile : MonoBehaviour
{
    private GridTile _currentGridTile;
    public GridTile CurrentGridTile
    {
        get { return _currentGridTile; }
        set { _currentGridTile = value; }
    }

    [SerializeField] private GameObject _fireVFXPrefab;

    private VFXManager _fireVFXRef;

    private bool isOnFire = false;
    public bool IsOnFire
    {
        get { return isOnFire; }
    }


    public void Ignite()
    {
        _fireVFXRef = Instantiate(_fireVFXPrefab, transform).GetComponent<VFXManager>();
        _fireVFXRef.EnableParticleEffectChildren();

        isOnFire = true;

        if (_currentGridTile.ObjectOccupyingTile.CompareTag(MetaConstants.CharacterTag))
        {
            _currentGridTile.ObjectOccupyingTile.GetComponent<Base_Ch>().HitByProjectile(Enum_ProjectileType.Fireball);
            GameTurnManager.Instance.AddDestroyedObject(_currentGridTile.ObjectOccupyingTile);
        }
        if (_currentGridTile.ObstacleImplementsScript())
        {
            Base_Obstacle baseObstacle = _currentGridTile.ObstacleImplementsScript();
            if (baseObstacle.CanBeDestroyed) 
            {
                baseObstacle.DisableObject();
                GameTurnManager.Instance.AddDestroyedObject(_currentGridTile.ObjectOccupyingTile);
            }

        }

        IgniteAnyNeighbours();
    }

    private void KillWizardOnTile()
    {

    }

    private void IgniteAnyNeighbours()
    {
        GridTile[] NeighbourTiles = _currentGridTile.FindNeighbouringTiles();

        for (int i = 1; i < NeighbourTiles.Length; i++)
        {
            if (NeighbourTiles[i].IsOilTile())
            {
                if (NeighbourTiles[i].GetComponent<OilTile>() && !NeighbourTiles[i].GetComponent<OilTile>().IsOnFire)
                {
                    NeighbourTiles[i].GetComponent<OilTile>().Ignite();
                }
            }
        }
    }

}
