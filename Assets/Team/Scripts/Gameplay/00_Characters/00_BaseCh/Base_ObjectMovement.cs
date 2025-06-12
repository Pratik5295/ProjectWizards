using UnityEngine;

public class Base_ObjectMovement : Base_Ch
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private ObstacleData _obstacleData;
    void Start()
    {
        _obstacleData = GetComponent<ObstacleData>();
        InitialiseCharacter(_obstacleData.CurrentTileID, Enum_GridDirection.NORTH);
    }
}
