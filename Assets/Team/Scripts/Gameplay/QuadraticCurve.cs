using UnityEngine;

public class QuadraticCurve : MonoBehaviour
{
    private Transform startPoint;
    private Transform endPoint;
    private Transform ControlPoint;

    private void Awake()
    {
        startPoint = new GameObject("Curve_StartPoint").transform;
        startPoint.transform.parent = transform;

        endPoint = new GameObject("Curve_EndPoint").transform;
        endPoint.transform.parent = transform;

        ControlPoint = new GameObject("Curve_ControlPoint").transform;
        ControlPoint.transform.parent = transform;
    }

    public Vector3 evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(startPoint.position, ControlPoint.position, t);
        Vector3 cb = Vector3.Lerp(ControlPoint.position, endPoint.position, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        if(!startPoint || !endPoint || !ControlPoint)
        {
            return;
        }

        for(int i = 0; i < 20; i++)
        {
            Gizmos.DrawWireSphere(evaluate(i / 20f), 0.1f);
        }
    }
}
