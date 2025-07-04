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
            LoadChaptersMap();
        }

        private void LoadChaptersMap()
        {
            foreach (var chapter in Chapters)
            {
                ChaptersMap.Add(chapter.chapterData.Data.ChapterID, chapter);
                chapter.Initialize();
            }

            Debug.Log("Loading all chapters complete...");

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

        }
    }
}
