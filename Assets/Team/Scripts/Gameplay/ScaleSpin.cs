using UnityEngine;
using DG.Tweening;

public class ScaleSpin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationDuration = 2f; // time it takes to complete one rotation

    [SerializeField]
    private Vector3 rotationAxis = new Vector3(0, 0, 360); // rotate around Y axis

    void Start()
    {
        Transform init_Transform = transform;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        transform.DOScale(Vector3.one , 0.5f);

        // Spins around the Y axis forever
        transform.DORotate(rotationAxis, rotationDuration, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Restart)
                 .SetEase(Ease.Linear); // linear for constant speed
    }
}
