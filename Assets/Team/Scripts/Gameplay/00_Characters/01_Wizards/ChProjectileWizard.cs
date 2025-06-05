using UnityEngine;

[DefaultExecutionOrder(2)]
public class ChProjectileWizard : Base_Ch
{
    [Header("Projectile Wizard Variables")]

    [Header("---Object References---")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _fireFromPoint;

    private GameObject ProjectileInstance;
    private Base_Projectile scProjectile;
    private QuadraticCurve curve;

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
    }


    public override void UseAbility()
    {
        ProjectileInstance = Instantiate(_projectilePrefab, _fireFromPoint.transform.position, Quaternion.identity);
        scProjectile = ProjectileInstance.GetComponent<Base_Projectile>();

        scProjectile.curve = curve;
        scProjectile.CastingWizard = this.gameObject;
        scProjectile.OnProjectileEnd += endTurn;
    }

    private void endTurn()
    {
        scProjectile.OnProjectileEnd -= endTurn;
        ProjectileInstance = null;
        scProjectile = null;

        OnTurnComplete();
    }
}
