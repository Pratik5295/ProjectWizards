using Team.Gameplay.GameLevelSystem;
using UnityEngine;

namespace Team.Managers
{
    [DefaultExecutionOrder(-20)]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance = null;

        public GameLevel CurrentLevel;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if(CurrentLevel != null)
            {
                var level = Instantiate(CurrentLevel);
                level.LoadLevel();
            }
        }
    }
}
