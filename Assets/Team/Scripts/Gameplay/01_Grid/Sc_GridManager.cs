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
    [SerializeField] private float gridY = 0.5f;   //Where to spawn the grid wrt height, Default is 0.5f
    [SerializeField] private float tileSize = 2f; //Width and height of the tile assuming each tile is square
    public Vector2 Max_GridSize_Acc
    {
        get { return Max_GridSize; }
    }
    [SerializeField] private float GridSlot_Offset;

    [SerializeField] private GameObject BasePrefab;
    [SerializeField] private GameObject HolderPrefab;

    [SerializeField]
    private List<GridSlot> Grid = new List<GridSlot>();
    public List<GridSlot> Grid_Acc
    {
        get{return Grid; }
    }

    private char[] gridCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

    [SerializeField]
    private GameObject ref_gridHolder;

    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        Grid.Clear();

        if (ref_gridHolder == null)
        {
            if (transform.childCount > 0)
            {
                ref_gridHolder = transform.GetChild(0).gameObject;
            }       
        }

        DestroyImmediate(ref_gridHolder);

        ref_gridHolder = null; //Just to avoid the "missing" feedback in inspector

    }

    //Creates Grid and fills up grid with references to tiles and locations.
    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        if (Grid.Count > 0 || ref_gridHolder != null || transform.childCount > 0)
        {
            Debug.LogError("Grid already exists, destroy the old grid first. No new grid created");
            return;
        }

        ref_gridHolder = Instantiate(HolderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        ref_gridHolder.transform.SetParent(transform);

        for (int x = 0; x < Max_GridSize.x; x++)
        {
            for (int y = 0; y < Max_GridSize.y; y++)
            {
                float positionX = x * (tileSize + GridSlot_Offset);
                float positionY = y * (tileSize + GridSlot_Offset);

                GridSlot newSlot = new GridSlot
                {
                    GridID = new Vector2(x, y),
                    tilePrefab = BasePrefab
                };
                Grid.Add(newSlot);

                var spawnedTile = SpawnTile(positionX, gridY, positionY);
                newSlot.tilePrefab = spawnedTile;
                newSlot.tilePrefab.name = $"Tile: {gridCharArray[x]} , {y}";
            }
        }
    }

    private GameObject SpawnTile(float x, float y, float z)
    {
        return Instantiate(BasePrefab,new Vector3(x,y, z), Quaternion.identity, ref_gridHolder.transform);
    }
}
