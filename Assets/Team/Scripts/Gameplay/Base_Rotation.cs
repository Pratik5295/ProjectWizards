using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Rotation : MonoBehaviour
{

    [SerializeField] private Enum_GridDirection dirFacing;
    public Enum_GridDirection DirectionFacing
    {
        get { return dirFacing; }
    }

    private Enum_GridDirection _previousDirFacing;

    private Dictionary<Enum_GridDirection, Vector2> _rotationDictionary = new Dictionary<Enum_GridDirection, Vector2>();
    private Vector2[] _rotationArray = new Vector2[4];

    public Dictionary<Enum_GridDirection, Vector2> RotationDictionary
    {
        get { return _rotationDictionary; }
    }

    private void Awake()
    {
       /* _rotationArray[0] = new Vector2(0, 1);
        _rotationArray[1] = new Vector2(0, -1);
        _rotationArray[2] = new Vector2(1, 0);
        _rotationArray[3] = new Vector2(-1, 0);*/

        foreach (Enum_GridDirection dir in (Enum_GridDirection[]) Enum.GetValues(typeof(Enum_GridDirection)))
        {
            switch (dir)
            {
                case Enum_GridDirection.NORTH:
                    _rotationDictionary.Add(dir, new Vector2(0, 1));
                    break;
                case Enum_GridDirection.SOUTH:
                    _rotationDictionary.Add(dir, new Vector2(0, -1));
                    break;
                case Enum_GridDirection.EAST:
                    _rotationDictionary.Add(dir, new Vector2(1, 0));
                    break;
                case Enum_GridDirection.WEST:
                    _rotationDictionary.Add(dir, new Vector2(-1, 0));
                    break;
            }
        }
    }

    public virtual void RotateToFaceDir(Vector2 dir)
    {
        _previousDirFacing = dirFacing;
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

    public Vector2 dirToV2(Enum_GridDirection direction)
    {
        Vector2 v2Direction;

        if (!_rotationDictionary.TryGetValue(dirFacing, out v2Direction))
        {
            Debug.LogError($"{gameObject.name} Base Rotation script couldn't output correct direction!");
            return new Vector2(0, 0);
        }
        return v2Direction;
    }

    public Vector2 GetFacingDirection()
    {
        Vector2 direction;

        /*return _rotationArray[(byte)dirFacing];*/
        
        if(!_rotationDictionary.TryGetValue(dirFacing, out direction))
        {
            Debug.LogError($"{gameObject.name} Base Rotation script couldn't output correct direction!");
            return new Vector2(0,0);
        }
        return direction;
    }

    public void changeFacingDirection(Enum_GridDirection Direction)
    {
        _previousDirFacing = dirFacing;
        dirFacing = Direction;
    }
}
