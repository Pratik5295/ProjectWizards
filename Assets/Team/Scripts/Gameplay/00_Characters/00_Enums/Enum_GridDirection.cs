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

public static class DirectionUtilities
{
    public static Enum_GridDirection RotateClockwise(Enum_GridDirection dir)
    {
        return (Enum_GridDirection)(((int)dir + 1) % 4);
    }
}
