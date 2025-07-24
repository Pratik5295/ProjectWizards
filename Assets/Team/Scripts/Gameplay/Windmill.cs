using DG.Tweening;
using UnityEngine;


public class Windmill : MonoBehaviour
{
    [SerializeField]
    private float duration = 5f;

    private Vector3 rot = new Vector3 (360f, 0f, 0f);

    Tween windmill;

    void Start()
    {
        windmill = transform.DORotate(rot, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void OnDisable()
    {
        windmill.Kill();
    }

}
