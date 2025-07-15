using System.Collections;
using DG.Tweening;
using Team.Data;
using TMPro;
using UnityEngine;

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
            StartCoroutine(PopulateCoroutine(_data));
        }

        private IEnumerator PopulateCoroutine(CharacterDataStruct _data)
        {
            if (isOpen)
            {
                // Close panel and wait for animation to finish
                yield return leftPanel.DOAnchorPosX(closedX, 0.2f).SetEase(Ease.InOutQuad).WaitForCompletion();
                isOpen = false;
            }

            // Set UI data
            characterNameText.text = _data.CharacterName;
            abilityNameText.text = _data.AbilityName;
            abilityDescriptionText.text = _data.AbilityDescription;

            // Open panel and wait if needed
            yield return leftPanel.DOAnchorPosX(openX, duration).SetEase(Ease.InOutQuad).WaitForCompletion();
            isOpen = true;
        }

        // Optional standalone open/close
        public void OpenPanel()
        {
            leftPanel.DOAnchorPosX(openX, duration).SetEase(Ease.InOutQuad);
            isOpen = true;
        }

        public void ClosePanel()
        {
            leftPanel.DOAnchorPosX(closedX, 0.1f).SetEase(Ease.InOutQuad);
            isOpen = false;
        }
    }
}
