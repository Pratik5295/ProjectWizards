using UnityEngine;

public class Base_ObjectMovement : Base_Ch
{
    [SerializeField]
    private ObstacleData _obstacleData;

    void Start()
    {
        _obstacleData = GetComponent<ObstacleData>();   
       
        InitialiseCharacter(_obstacleData.CurrentTileID, Enum_GridDirection.NORTH);
    }
}
