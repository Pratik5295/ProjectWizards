using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Team.Enum.Character
{
    public interface IProjectileHittable
    {

        void HitByProjectile(Enum_ProjectileType projectileType);
    }
}
