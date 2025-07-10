using UnityEngine;
using Team.Enum.Character;
using Team.Managers;


public class Base_Projectile : MonoBehaviour
{
    [SerializeField] protected Enum_ProjectileType _projectileType;
    [SerializeField] public Enum_GridDirection _projectileDir;

    [SerializeField] protected GameObject _VFX;

    public GameObject CastingWizard;
    public GameObject _prefabReference;


    [Header("Curve, Speed and Timing Variables")]
    public QuadraticCurve curve;
    [SerializeField] private float _speed = 1f;

    private float time;
    [SerializeField] private float _lifespan = 2f;

    float t = 0f;

    protected bool canMove = true;

    [Header("Particle Effects")]
    [SerializeField] protected ParticleSystem _collisionEffect;

    public System.Action OnProjectileEnd;

    void Start()
    {
        ProjectileManager.Instance.RegisterSceneProjectile(this);
        time = 0f;
        _VFX = transform.GetChild(0).gameObject;
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

        if (other.GetComponent<ChRedirectWizard>())
        {
            other.GetComponent<ChRedirectWizard>().TryAbsorbProjectile(_projectileType, _prefabReference, _projectileDir, 1);
        }

        if (_collisionEffect) { _collisionEffect.Play(); }

        CleanUp();
    }

    public virtual void CleanUp()
    {
        ProjectileManager.Instance.UnregisterSceneProjectile(this);
        OnProjectileEnd?.Invoke();
        Destroy(gameObject);
    }
}
