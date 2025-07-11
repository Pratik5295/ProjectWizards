using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team.Gameplay.GridSystem;
using Team.Managers;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class FireSpread : MonoBehaviour
{
    public static FireSpread Instance;

    public FireballProjectile FireballRef;

    [SerializeField]
    private LevelTileCreator _levelTileCreator;

    private float gridWidth, gridHeight;

    int iteration = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        _levelTileCreator = LevelManager.Instance.CreatedLevel.LevelTiles;
        gridWidth = _levelTileCreator.Max_GridSize_Acc.x;
        gridHeight = _levelTileCreator.Max_GridSize_Acc.y;
    }

    [ContextMenu("Start A Fire!")]
    public void StartFire()
    {
        StartCoroutine(SpreadFire());
    }

    public IEnumerator SpreadFire()
    {
        Queue<OilTile> fireQueue = new Queue<OilTile>();

        for (int x = 0; x <  _levelTileCreator.OilTiles.Count; x++)
        {
             fireQueue.Enqueue(_levelTileCreator.OilTiles[x]);
        }

        while (fireQueue.Count > 0)
        {
            OilTile currentTile = fireQueue.Dequeue();

            GridTile[] neighbours = currentTile.FindNeighbouringTiles();

            for (int i = 1; i < neighbours.Length; i++)
            {
                if (neighbours[i] && neighbours[i].GetComponent<OilTile>())
                {
                    OilTile neighbouringOilTile = neighbours[i].GetComponent<OilTile>();                

                    if(!neighbouringOilTile.visited && !neighbouringOilTile.isOnFire)
                    {
                        yield return new WaitForSeconds(0.2f);
                        neighbouringOilTile.Ignite();
                        fireQueue.Enqueue(neighbouringOilTile);
                        neighbouringOilTile.visited = true;
                    }
                    if (neighbouringOilTile.visited && neighbouringOilTile.isOnFire)
                    {
                        continue;
                    }
                }
            }
        }
        yield return new WaitForSeconds(2f);
        ExtinguishFire();
    }

    public void ExtinguishFire()
    {
        for (int i = 0; i < _levelTileCreator.OilTiles.Count; i++)
        {
            if (_levelTileCreator.OilTiles[i].visited)
            {
                if (!_levelTileCreator.OilTiles[i].isOnFire)
                {
                    _levelTileCreator.OilTiles[i].visited = false;
                }
                else if(_levelTileCreator.OilTiles[i].isOnFire)
                    _levelTileCreator.OilTiles[i].Extinguish();
            }
        }
        iteration = 0;

        FireballRef?.CleanUp();
        FireballRef = null;
    }

    public void ResetOilTiles()
    {
        for(int i = 0; i < _levelTileCreator.OilTiles.Count; i++)
        {
            _levelTileCreator.OilTiles[i].ResetOilStatus();
        }
        iteration = 0;
    }
}
