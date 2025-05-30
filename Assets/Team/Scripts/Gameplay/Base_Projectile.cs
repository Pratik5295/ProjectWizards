using UnityEngine;

public class Base_Projectile : MonoBehaviour
{

    public QuadraticCurve curve;
    [SerializeField] private float _speed = 1f;

    private float time;
    [SerializeField] private float _lifespan = 2f;

    void Start()
    {
        time = 0f;
    }


    void Update()
    {
        time += Time.deltaTime * _speed;
        transform.position = curve.evaluate(time);
        transform.forward = curve.evaluate(time + 0.001f) - transform.position;

        if(time >= _lifespan)
        {
            CleanUp();
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public virtual void CleanUp()
    {
        Destroy(gameObject);
    }
}
