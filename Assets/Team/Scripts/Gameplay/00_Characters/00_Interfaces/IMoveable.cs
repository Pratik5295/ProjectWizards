using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable 
{
    IEnumerator MoveByAmount(int movementAmount, Vector2 dir);
    IEnumerator LerpingMovement(Vector3 targetPosition);
}
