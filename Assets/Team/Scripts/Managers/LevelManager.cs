using System;
using System.Collections.Generic;
using Team.Gameplay.LevelSystem;
using UnityEngine;

namespace Team.Managers
{
    [DefaultExecutionOrder(-20)]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance = null;

        public List<LevelInfoSO> LevelMap = new List<LevelInfoSO>();

        public LevelInfoSO CurrentLevelSO;

        public Action<LevelInfoSO> OnCurrentLevelUpdated;

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

        public void SetCurrentLevel(LevelInfoSO _level)
        {
            CurrentLevelSO = _level;
            OnCurrentLevelUpdated?.Invoke(CurrentLevelSO);
        }

        [ContextMenu("Load Current Level")]
        public void LoadCurrentLevel()
        {
            if (CurrentLevelSO != null)
            {
                var level = Instantiate(CurrentLevelSO.GameLevelPrefab);
                level.LoadLevel();
            }
        }
    }
}
