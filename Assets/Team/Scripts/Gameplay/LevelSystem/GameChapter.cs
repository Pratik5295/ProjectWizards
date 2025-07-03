using System.Collections.Generic;
using UnityEngine;
using static Team.GameConstants.LevelConstants;
using static Team.GameConstants.MetaConstants;

namespace Team.Gameplay.LevelSystem
{
    public class GameChapter : MonoBehaviour
    {
        public ChapterState Status;

        public ChapterDataSO chapterData;

        public List<LevelID> CompletedLevels = new List<LevelID>();

        public List<Level> LevelObjects = new List<Level>();

        public int LevelsCompleted => CompletedLevels.Count;

        #region Unity Methods

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        #endregion

        #region Level Completion Status

        public void AddCompletedLevel(LevelID levelID)
        {
            if (CompletedLevels.Contains(levelID))
            {
                Debug.LogWarning($"Level: {levelID} has already been completed and added");
                return;
            }

            CompletedLevels.Add(levelID);
        }

        public bool IsLevelCompleted(LevelID _levelID)
        {
            return CompletedLevels.Contains(_levelID);
        }

        #endregion


        #region Level Objects Region

        public void AddLevel(Level level)
        {
            if (LevelObjects.Contains(level)) return;

            LevelObjects.Add(level);

            //Adding listener
            level.OnCompletedLevel += OnLevelCompleted;
        }

        private void UnSubscribeEvents()
        {
            foreach (Level level in LevelObjects)
            {
                level.OnCompletedLevel -= OnLevelCompleted;
            }
        }

        /// <summary>
        /// Connect this listener to the event being fired from the 
        /// </summary>
        /// <param name="_levelID"></param>
        public void OnLevelCompleted(LevelID _levelID)
        {
            AddCompletedLevel(_levelID);
        }

        #endregion
    }
}
