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
        [Header("Panel & Animation")]
        public RectTransform leftPanel;
        public float duration = 0.5f;
        private float openX = 250f;
        private float closedX = -180f;

        [Header("Text & Video References")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI abilityNameText;
        [SerializeField] private TextMeshProUGUI abilityDescriptionText;
        [SerializeField] private VideoPlayer gameplayVideo;

        [Header("State Tracking")]
        [SerializeField] private bool isOpen = true;
        [SerializeField] private string _cacheCharacterName;

        private Tween currentTween = null;
        private Coroutine activeCoroutine = null;
        private bool isLocked = false;

        public void Populate(CharacterDataStruct _data)
        {
            if (isLocked) return;
            isLocked = true;

            // First time opening
            if (string.IsNullOrEmpty(_cacheCharacterName))
            {
                activeCoroutine = StartCoroutine(OpenPanel(_data));
            }
            // Same character clicked — close panel
            else if (string.Equals(_cacheCharacterName, _data.CharacterName))
            {
                activeCoroutine = StartCoroutine(ClosePanel());
            }
            // Different character — switch panels
            else
            {
                activeCoroutine = StartCoroutine(SwapPanel(_data));
            }
        }

        private IEnumerator SwapPanel(CharacterDataStruct _data)
        {
            KillCurrentTween();

            // Close the current panel
            currentTween = leftPanel.DOAnchorPosX(closedX, 0.2f).SetEase(Ease.InOutQuad);
            yield return currentTween.WaitForCompletion();

            isOpen = false;
            _cacheCharacterName = string.Empty;

            // Open new panel
            yield return OpenPanel(_data);
        }

        private IEnumerator OpenPanel(CharacterDataStruct _data)
        {
            _cacheCharacterName = _data.CharacterName;

            // Set content
            characterNameText.text = _data.CharacterName;
            abilityNameText.text = _data.AbilityName;
            abilityDescriptionText.text = _data.AbilityDescription;

            gameplayVideo.clip = _data.AbilityVideo;
            gameplayVideo.Play();

            KillCurrentTween();

            // Animate panel in
            currentTween = leftPanel.DOAnchorPosX(openX, duration).SetEase(Ease.InOutQuad);
            yield return currentTween.WaitForCompletion();

            isOpen = true;
            

            activeCoroutine = null;
            isLocked = false;
        }

        public IEnumerator ClosePanel()
        {
            KillCurrentTween();

            currentTween = leftPanel.DOAnchorPosX(closedX, 0.1f).SetEase(Ease.InOutQuad);
            yield return currentTween.WaitForCompletion();

            isOpen = false;
            _cacheCharacterName = string.Empty;

            activeCoroutine = null;
            isLocked = false;
        }

        private void KillCurrentTween()
        {
            if (currentTween != null && currentTween.IsActive())
            {
                currentTween.Kill();
                currentTween = null;
            }
        }
    }
}
