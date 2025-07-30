using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enum_GridDirection
{
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3
}

/// <summary>
/// Singleton for Direction based utilities: includes returning an Enum based on clockwise or anti clockwise rotation.
/// </summary>
public static class DirectionUtilities
{
    public static Enum_GridDirection RotateClockwise(Enum_GridDirection dir)
    {
        return (Enum_GridDirection)(((int)dir + 1) % 4);
    }

    public static Enum_GridDirection RotateAntiClockwise(Enum_GridDirection dir)
    {
        return (Enum_GridDirection)(((int)dir + 3) % 4);
    }

    public static float DirectionRotation(Enum_GridDirection dir)
    {
        switch (dir)
        {
            case Enum_GridDirection.NORTH:
                return 0;
                

            case Enum_GridDirection.EAST:
                return 90;
                

            case Enum_GridDirection.SOUTH:
                return 180;
                

            case Enum_GridDirection.WEST:
                return 270;
                
        }
        return 0;
    }
}
