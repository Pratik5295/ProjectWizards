using Team.Data;
using TMPro;
using UnityEngine;

namespace Team.UI
{
    public class UIInforPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI characterNameText;

        [SerializeField]
        private TextMeshProUGUI abilityNameText;

        [SerializeField]
        private TextMeshProUGUI abilityDescriptionText;

        public void Populate(CharacterDataStruct _data)
        {
            characterNameText.text = _data.CharacterName;
            abilityNameText.text = _data.AbilityName;
            abilityDescriptionText.text = _data.AbilityDescription;
        }
    }
}
