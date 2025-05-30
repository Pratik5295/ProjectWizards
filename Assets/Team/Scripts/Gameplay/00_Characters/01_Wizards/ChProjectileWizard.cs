using UnityEngine;

public class ChProjectileWizard : Base_Ch
{
    [Header("Projectile Wizard Variables")]

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _fireFromPoint;

    private GameObject ProjectileInstance;



    public override void UseAbility()
    {
        ProjectileInstance = Instantiate(_projectilePrefab, _fireFromPoint.transform.position, Quaternion.identity);
        ProjectileInstance.GetComponent<Base_Projectile>().curve = _fireFromPoint.GetComponent<QuadraticCurve>();
    }
}
