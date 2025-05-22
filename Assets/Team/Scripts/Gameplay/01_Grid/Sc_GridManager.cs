using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSlot
{
    public Vector2 GridID;

    public GameObject tilePrefab;

}

public class Sc_GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 Max_GridSize;
    public Vector2 Max_GridSize_Acc
    {
        get { return Max_GridSize; }
    }
    [SerializeField] private Vector2 GridSlot_Offset;
    public Vector2 GridSlot_Offset_Acc
    {
        get { return GridSlot_Offset; }
    }

    [SerializeField] private GameObject BasePrefab;
    [SerializeField] private GameObject HolderPrefab;


    private List<GridSlot> Grid = new List<GridSlot>();
    public List<GridSlot> Grid_Acc
    {
        get{return Grid; }
    }

    private char[] gridCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

    GameObject ref_gridHolder;

    private void OnEnable()
    {
        //CreateGrid();
    }

    private void Awake()
    {
        CreateGrid();
    }

    public void RefreshGrid()
    {
        Grid.Clear();
        Destroy(ref_gridHolder);
        CreateGrid();
    }

    //Creates Grid and fills up grid with references to tiles and locations.
    void CreateGrid()
    {
        ref_gridHolder = Instantiate(HolderPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        for(int x = 0; x < Max_GridSize.x; x++)
        {
            for(int y = 0; y < Max_GridSize.y; y++) //This is the reason why the tiles generate Y before X. Because Y loop completes firstly then moves onto next X axis.
            {
                GridSlot newSlot = new GridSlot();
                newSlot.GridID = new Vector2(x, y);
                if(BasePrefab != null)
                {
                    newSlot.tilePrefab = BasePrefab;
                }
                Grid.Add(newSlot);

                Collider[] grabbedWorldTiles = Physics.OverlapSphere(new Vector3(newSlot.GridID.x, 0, newSlot.GridID.y), 0.5f);
                GameObject selectedTile = grabbedWorldTiles[0].gameObject;
                newSlot.tilePrefab = selectedTile;
                newSlot.tilePrefab.name = $"Tile: {gridCharArray[x]} , {y}";

                /*var spawnedTile = Instantiate(selectedTile, 
                    new Vector3(newSlot.GridID.x * GridSlot_Offset.x, 10, newSlot.GridID.y * GridSlot_Offset.y), Quaternion.identity, ref_gridHolder.transform);

                spawnedTile.name = $"Tile: {gridCharArray[x]} , {y}";*/
            }
        }
    }
}
