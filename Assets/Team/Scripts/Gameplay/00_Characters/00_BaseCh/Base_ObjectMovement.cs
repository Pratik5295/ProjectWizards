using UnityEngine;

public class Base_ObjectMovement : Base_Ch
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialiseCharacter(_currentTileID, Enum_GridDirection.NORTH);
    }
}
