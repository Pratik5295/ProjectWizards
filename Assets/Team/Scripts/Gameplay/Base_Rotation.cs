using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Rotation : MonoBehaviour
{

    [SerializeField] private Enum_GridDirection dirFacing;

    public virtual void RotateToFaceDir(Vector2 dir)
    {
        switch (dir.x)
        {
            case 1:
                dirFacing = Enum_GridDirection.EAST;
                transform.eulerAngles = new Vector3(transform.rotation.x, 90, transform.rotation.z);
                return;

            case -1:
                dirFacing = Enum_GridDirection.WEST;
                transform.eulerAngles = new Vector3(transform.rotation.x, 270, transform.rotation.z);
                return;
            case 0:
                break;
        }
        switch (dir.y)
        {
            case 1:
                dirFacing = Enum_GridDirection.NORTH;
                transform.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.z);
                return;

            case -1:
                dirFacing = Enum_GridDirection.SOUTH;
                transform.eulerAngles = new Vector3(transform.rotation.x, 180, transform.rotation.z);
                return;
            case 0:
                break;
        }
    }

}
