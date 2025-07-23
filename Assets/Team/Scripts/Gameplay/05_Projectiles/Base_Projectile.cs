using UnityEngine;
using Team.Enum.Character;
using Team.Managers;
using Team.Gameplay.GridSystem;

public class ProjectileHandler
{
    public Enum_ProjectileType ProjectileType;
    public Base_Ch characterInteracted;
    public Base_Obstacle obstacleInteracted;
    public TileID beforeTileID;

}


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


    public ProjectileHandler _projectileHandle = null;
    public System.Action<ProjectileHandler> OnProjectileEnd;

    void Start()
    {
        ProjectileManager.Instance.RegisterSceneProjectile(this);
        _projectileHandle = new ProjectileHandler();
        _projectileHandle.ProjectileType = _projectileType;

        time = 0f;
        _VFX = transform.GetChild(0).gameObject;
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
                Debug.Log($"Pratik Projectile: {gameObject.name} dying out after lifespan");
                ResetProjectileHandler();
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
        OnProjectileEnd?.Invoke(_projectileHandle);
        Destroy(gameObject);
    }

    protected void ResetProjectileHandler()
    {
        _projectileHandle = null;
    }

    protected void VisuallyDestroy()
    {
        canMove = false;
        _VFX.SetActive(false);
    }
}
