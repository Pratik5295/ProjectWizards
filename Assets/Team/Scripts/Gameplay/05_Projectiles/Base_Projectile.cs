using UnityEngine;
using Team.Enum.Character;
using Team.GameConstants;


public class Base_Projectile : MonoBehaviour
{
    [SerializeField] protected Enum_ProjectileType _projectileType;
    [SerializeField] public Enum_GridDirection _projectileDir;

    public GameObject CastingWizard;


    [Header("Curve, Speed and Timing Variables")]
    public QuadraticCurve curve;
    [SerializeField] private float _speed = 1f;

    private float time;
    [SerializeField] private float _lifespan = 2f;

    float t = 0f;

    [Header("Particle Effects")]
    [SerializeField] protected ParticleSystem _collisionEffect;

    public System.Action OnProjectileEnd;

    void Start()
    {
        time = 0f;
    }


    void Update()
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

        if(t >= 1f || time >= _lifespan)
        {
            CleanUp();
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {

        if (_collisionEffect) { _collisionEffect.Play(); }

        OnProjectileEnd();
        Destroy(this.gameObject);
    }

    public virtual void CleanUp()
    {
        OnProjectileEnd();
        Destroy(gameObject);
    }
}
