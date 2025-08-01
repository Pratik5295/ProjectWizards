using System.Collections;
using DG.Tweening;
using Team.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Team.UI
{
    public class UIInforPanel : MonoBehaviour
    {
        public RectTransform leftPanel;
        public float duration = 0.5f;

        [SerializeField]
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

        [SerializeField]
        private VideoPlayer gameplayVideo;

        [SerializeField]
        private string _cacheCharacterName;

        private Coroutine activeCoroutine = null;

        public void Populate(CharacterDataStruct _data)
        {
            if (activeCoroutine != null) return;

            if (string.IsNullOrEmpty(_cacheCharacterName))
            {
                activeCoroutine = StartCoroutine(HandleOpeningPanel(_data));

            }
            else
            {
                //Has name, check if its the same data file
                if (string.Equals(_cacheCharacterName, _data.CharacterName))
                {
                    activeCoroutine = StartCoroutine(ClosePanel());
                }
                else
                {
                    //Opening different character, close current panel and open new one
                    activeCoroutine =  StartCoroutine(PopulateCoroutine(_data));
                }
            }
          
        }

        private IEnumerator PopulateCoroutine(CharacterDataStruct _data)
        {
            if (isOpen)
            {
                // Close panel and wait for animation to finish
                yield return leftPanel.DOAnchorPosX(closedX, 0.2f).SetEase(Ease.InOutQuad).WaitForCompletion();

                activeCoroutine = StartCoroutine(HandleOpeningPanel(_data));
                isOpen = false;

            }
            else
            {
                // Open panel and wait if needed
                yield return activeCoroutine = StartCoroutine(HandleOpeningPanel(_data));
            }
        }

        private IEnumerator HandleOpeningPanel(CharacterDataStruct _data)
        {
            characterNameText.text = _data.CharacterName;
            abilityNameText.text = _data.AbilityName;
            abilityDescriptionText.text = _data.AbilityDescription;

            gameplayVideo.clip = _data.AbilityVideo;

            // Open panel and wait if needed
            yield return leftPanel.DOAnchorPosX(openX, duration).SetEase(Ease.InOutQuad).WaitForCompletion();

            isOpen = true;
            _cacheCharacterName = _data.CharacterName;

            gameplayVideo.Play();

            activeCoroutine = null;
        }

        public IEnumerator ClosePanel()
        {
            yield return leftPanel.DOAnchorPosX(closedX, 0.1f).SetEase(Ease.InOutQuad).WaitForCompletion();

            isOpen = false;
            _cacheCharacterName = string.Empty;

            activeCoroutine = null;
        }
    }
}
