using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileHittable
{

    void HitByProjectile(Enum_ProjectileType projectileType);
}
