using UnityEngine;

public class QuadraticCurve : MonoBehaviour
{
    public Transform startPoint;

    public Transform endPoint;

    public Transform controlPoint;

    public Vector3 evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(startPoint.position, controlPoint.position, t);
        Vector3 cb = Vector3.Lerp(controlPoint.position, endPoint.position, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        if(!startPoint || !endPoint || !controlPoint)
        {
            return;
        }

        for(int i = 0; i < 20; i++)
        {
            Gizmos.DrawWireSphere(evaluate(i / 20f), 0.1f);
        }
    }
}
