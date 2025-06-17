using TMPro;
using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.Gameplay.LevelSystem
{
    public class LevelInfoUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI levelNameText;

        [SerializeField]
        private LevelInfoData levelInfoData;

        public bool IsUnlocked => levelInfoData.State == LevelState.UNLOCKED;

        public void PopulateLevelInfo(LevelInfoData _data)
        {
            levelInfoData = _data;

            levelNameText.text = levelInfoData.LevelName;
        }

        public void OnLevelSelected()
        {
            if (IsUnlocked)
            {
                //Unlocked, allow to play level
            }
            else
            {
                //Locked
            }
        }
    }
}
