using Team.Data;
using Team.GameConstants;
using TMPro;
using UnityEngine;


namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float BarkHideAfter = 1f;
    }
}


namespace Team.UI
{

    public class UICharacter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI characterNameText;

        [SerializeField]
        private GameObject barkObject;

        [SerializeField]
        private TextMeshProUGUI characterBarkText;


        public void PopulateCharacterUI(string _characterName, CharacterReskinData _reskinData)
        {
            characterNameText.text = _characterName;
            characterNameText.color = _reskinData.CharacterColor;
        }

        private void ShowBarkUI()
        {
            barkObject.SetActive(true);

            Invoke(nameof(HideBarkUI),MetaConstants.BarkHideAfter);
        }

        private void HideBarkUI()
        {
            barkObject.SetActive(false);
        }

        public void UpdateBark(string _barkMessage)
        {
            characterBarkText.text = _barkMessage;

            ShowBarkUI();
        }
    }
}
