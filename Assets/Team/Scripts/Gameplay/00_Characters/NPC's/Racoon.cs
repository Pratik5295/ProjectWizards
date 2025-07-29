using Team.Enum.Character;
using Team.Managers;
using UnityEngine;

public class Racoon : MonoBehaviour
{
    [SerializeField] protected GameObject _mesh;
    [SerializeField] protected GameObject _VFX;

    [Header("Curve, Speed and Timing Variables")]
    public QuadraticCurve curve;
    [SerializeField] private float _speed = 1f;

    private float time;
    [SerializeField] private float _lifespan = 2f;

    float t = 0f;

    protected bool canMove = true;

    [Header("Particle Effects")]
    [SerializeField] protected ParticleSystem _collisionEffect;

    void Start()
    {
        time = 0f;
        _mesh = transform.GetChild(0).gameObject;  
        _VFX = transform.GetChild(1).gameObject;
        FiredSound();
    }

    public virtual void FiredSound()
    {

    }

    void Update()
    {
        if (canMove)
        {
            time += Time.deltaTime * _speed;
            t = Mathf.Clamp01(time / _lifespan);

            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = curve.evaluate(t);

            Vector3 evaluatedPosition = curve.evaluate(Mathf.Clamp01(t + 0.001f));
            Vector3 direction = evaluatedPosition - currentPosition;

            transform.position = nextPosition;

            //Orient the projectile, if the direction is valid.
            if (direction != Vector3.zero)
            {
                transform.forward = direction.normalized;
            }

            if (t >= 1f || time >= _lifespan)
            {
                CleanUp();
            }
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    { 
        if (_collisionEffect) { _collisionEffect.Play(); }

        CleanUp();
    }

    public virtual void CleanUp()
    {
        Destroy(curve.gameObject);
        EnvrionmentalsManager.Instance.ExecuteRocketsSpawnLoop();
        Destroy(gameObject);
    }
}
