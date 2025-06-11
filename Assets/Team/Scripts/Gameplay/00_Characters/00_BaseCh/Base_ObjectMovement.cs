using Team.Gameplay.GridSystem;
using UnityEngine;

public class Base_ObjectMovement : Base_Ch
{
    [SerializeField]
    private ObstacleData _obstacleData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _obstacleData = GetComponent<ObstacleData>();   
       
        InitialiseCharacter(_obstacleData.CurrentTileID, Enum_GridDirection.NORTH);
    }
}
