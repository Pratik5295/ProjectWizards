using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Team.UI
{
    public struct UIChapterInfo
    {
        public string ChapterTitle;
        public Sprite ChapterImage;
        public Sprite ChapterNumber;

        public UIChapterInfo(string _title,Sprite _chapterImage, Sprite _chapterNumber)
        {
            ChapterTitle = _title;
            ChapterImage = _chapterImage;
            ChapterNumber = _chapterNumber;
        }
    }

    /// <summary>
    /// This script will be attached to the UI parent of the chapter
    /// The script will be responsible for changing the image and chapter title
    /// to allow the illusion of cycling through different chapters
    /// </summary>
    public class UIChapter : MonoBehaviour
    {
        [SerializeField]
        private Image chapterImage;

        [SerializeField]
        private TextMeshProUGUI chapterName;

        [SerializeField]
        private Image chapterNumberImage;

        public void PopulateChapterInfo(UIChapterInfo _info)
        {
            chapterImage.sprite = _info.ChapterImage;
            chapterName.text = _info.ChapterTitle;
            chapterNumberImage.sprite = _info.ChapterNumber;
        }
    }
}
