using System.Collections.Generic;
using Team.Managers;
using UnityEngine;
using static Team.GameConstants.LevelConstants;

namespace Team.Gameplay.LevelSystem
{
    [DefaultExecutionOrder(-15)]
    public class ChapterManager : MonoBehaviour
    {
        public static ChapterManager Instance = null;

        public List<GameChapter> Chapters = new List<GameChapter>();    //To be loaded by the designer or via an SO in the future

        public Dictionary<ChapterID,GameChapter> ChaptersMap = new Dictionary<ChapterID, GameChapter>();

        

        private void Awake()
        {
            if (Instance == null)
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
            //LoadChaptersMap();
        }


        private void LoadChaptersMap()
        {
            //TODO: Put this in UNitask async at start in the future, so the chapter data is never empty
            foreach (var chapter in Chapters)
            {
                ChaptersMap.Add(chapter.chapterData.Data.ChapterID, chapter);
                chapter.Initialize();
            }
            LevelManager.Instance.LoadLevelMap();
        }

        [ContextMenu("Get All Chapter Details")]
        public void GetAllDetails()
        {
            foreach(var chapter in ChaptersMap)
            {
                Debug.Log($"Chapter: {chapter.Key}, State: {chapter.Value.Status}");
            }
        }

        public void OnChapterCompleted(GameChapter _chapter)
        {
            ChapterID nextChapterId = _chapter.NextChapterID;

            GameChapter nextChapter = ChaptersMap[nextChapterId];

            nextChapter.UnlockChapter();
        }

        #region Load Game Section

        public void LoadSaveData(List<SavePacket> _dataPackets)
        {
            foreach(var packet in _dataPackets)
            {
                //Find the relevant Game chapter via id
                var chapter = ChaptersMap[packet.ChapterID];
                chapter.OnChapterCompletedLevels(packet.CompletedLevels);

            }
        }

        #endregion
    }
}
