using System.Collections.Generic;
using UnityEngine;

namespace Team.Managers
{
    public class ProjectileManager : MonoBehaviour
    {
        public static ProjectileManager Instance;

        [SerializeField]
        private List<Base_Projectile> _sceneProjectiles = new List<Base_Projectile>();


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(gameObject);
        }

        public void RegisterSceneProjectile(Base_Projectile Projectile)
        {
            _sceneProjectiles.Add(Projectile);
        }

        public void UnregisterSceneProjectile(Base_Projectile Projectile)
        {
            _sceneProjectiles.Remove(Projectile);
        }

        public void ImmediateCleanUp()
        {
            for (int i = 0; i < _sceneProjectiles.Count; i++)
            {
                _sceneProjectiles[i].CleanUp();
            }
        }

    }
}
