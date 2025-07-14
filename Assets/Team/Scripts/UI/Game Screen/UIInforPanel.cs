using Team.Data;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Team.UI
{
    public class UIInforPanel : MonoBehaviour
    {
        public RectTransform leftPanel;
        public float duration = 0.5f;

        private bool isOpen = true;

        // Positions
        private float openX = 250f;
        private float closedX = -180f;

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

            OpenPanel();
        }

        // Optional: Open/Close manually
        public void OpenPanel()
        {
            //leftPanel.DOAnchorPosX(openX, duration).SetEase(Ease.InOutQuad);
            isOpen = true;
        }

        public void ClosePanel()
        {
            //leftPanel.DOAnchorPosX(closedX, duration).SetEase(Ease.InOutQuad);
            isOpen = false;
        }
    }
}
