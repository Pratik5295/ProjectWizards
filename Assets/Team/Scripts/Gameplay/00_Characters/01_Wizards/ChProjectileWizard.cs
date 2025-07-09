using UnityEngine;

[DefaultExecutionOrder(2)]
public class ChProjectileWizard : Base_Ch
{
    [Header("Projectile Wizard Variables")]

    [Header("---Object References---")]
    [SerializeField] protected GameObject _projectilePrefab;
    public GameObject ProjectilePrefab
    {
        set { _projectilePrefab = value; }
        get { return _projectilePrefab; }
    }
    [SerializeField] private GameObject _fireFromPoint;

    private GameObject ProjectileInstance;
    private Base_Projectile scProjectile;
    private QuadraticCurve curve;

    public ProjectileGhosting ghostingEffect;

    [Header("---Wizard Projectile Stats---")]
    [Range(-1, -300)]
    [SerializeField] private int _projectileRange = -1;
    

    void Awake()
    {
        curve = _fireFromPoint.GetComponent<QuadraticCurve>();

        curve.startPoint = new GameObject("Curve_StartPoint").transform;
        curve.startPoint.transform.parent = curve.transform;
        curve.startPoint.transform.localPosition = new Vector3(0, 0, 0);

        curve.endPoint = new GameObject("Curve_EndPoint").transform;
        curve.endPoint.transform.parent = curve.transform;
        curve.endPoint.localPosition = new Vector3(_projectileRange, 0, 0);

        curve.controlPoint = new GameObject("Curve_ControlPoint").transform;
        curve.controlPoint.transform.parent = curve.transform;
        curve.controlPoint.localPosition = new Vector3(_projectileRange/2, 0, 0);

        if (transform.GetComponentInChildren<ProjectileGhosting>())
        {
            ghostingEffect = transform.GetComponentInChildren<ProjectileGhosting>();
            ghostingEffect.SetProjectionValue(_projectileRange);
        }
    }


    public override void UseAbility()
    {
        if (!_projectilePrefab) 
        { 
            StartCoroutine(ShakeCharacter(0.25f)); 
            endTurn(); 
            return; 
        }
        ProjectileInstance = Instantiate(_projectilePrefab, _fireFromPoint.transform.position, Quaternion.identity);
        scProjectile = ProjectileInstance.GetComponent<Base_Projectile>();

        scProjectile.curve = curve;
        scProjectile.CastingWizard = this.gameObject;
        scProjectile._prefabReference = _projectilePrefab;
        scProjectile._projectileDir = baseRotation.DirectionFacing;
        scProjectile.OnProjectileEnd += endTurn;

        OnCastBark();
    }

    private void endTurn()
    {
        if (_projectilePrefab)
        {
            scProjectile.OnProjectileEnd -= endTurn;
        }
        ProjectileInstance = null;
        scProjectile = null;

        OnTurnComplete();
    }
}
