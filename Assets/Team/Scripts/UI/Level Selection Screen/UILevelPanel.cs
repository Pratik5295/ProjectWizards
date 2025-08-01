using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Team.UI
{

    public class UILevelPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI levelTitle;

        [SerializeField]
        private TextMeshProUGUI levelDescription;

        [SerializeField]
        private TextMeshProUGUI primaryObjective;

        [SerializeField]
        private TextMeshProUGUI secondaryObjective;

        [SerializeField]
        private GameObject secondaryObjectiveObject;
        [SerializeField]
        private GameObject secondaryObjectiveHolder;

        //Tweening holder
        public RectTransform targetPanel;
        public float duration = 0.5f;
        public float offset = 800f; // how far it moves in/out
        
        private Vector2 originalAnchoredPosition;
        private bool isOpen = false;
        private Tween currentTween;

        private void Start()
        {
            originalAnchoredPosition = targetPanel.anchoredPosition;

            // Start off-screen to the right
            Vector2 hiddenRight = originalAnchoredPosition + new Vector2(offset, 0);
            targetPanel.anchoredPosition = hiddenRight;

            TweenOut();
        }


        public void PopulateData(string _title, string _description, List<string> _objectives, bool forceRefresh = true)
        {
            if (forceRefresh)
                ToggleOpen();
            else if (!isOpen)
                TweenIn();

            levelTitle.text = _title;
            levelDescription.text = _description;

            primaryObjective.text = _objectives[0];

            if (_objectives.Count > 1)
            {
                secondaryObjectiveObject.SetActive(true);
                secondaryObjectiveHolder.SetActive(true);
                secondaryObjective.text = _objectives[1];
            }
            else
            {
                secondaryObjectiveObject.SetActive(false);
                secondaryObjectiveHolder.SetActive(false);
            }
        }


        public void TweenIn()
        {
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

            targetPanel.DOAnchorPos(originalAnchoredPosition, duration)
                .SetEase(Ease.OutCubic);

            isOpen = true;
        }

        public void TweenOut()
        {
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

            Vector2 hiddenPos = originalAnchoredPosition + new Vector2(offset, 0);
            targetPanel.DOAnchorPos(hiddenPos, duration)
                .SetEase(Ease.InCubic);

            isOpen = false;
        }

        public void ToggleOpen()
        {
            if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

            Vector2 hiddenPos = originalAnchoredPosition + new Vector2(offset, 0);

            if (isOpen)
            {
                // Close and reopen
                currentTween = targetPanel.DOAnchorPos(hiddenPos, duration / 2)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        currentTween = targetPanel.DOAnchorPos(originalAnchoredPosition, duration)
                            .SetEase(Ease.OutCubic)
                            .OnComplete(() => isOpen = true);
                    });
            }
            else
            {
                // Just open
                currentTween = targetPanel.DOAnchorPos(originalAnchoredPosition, duration)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() => isOpen = true);
            }

            isOpen = false; // Assume it’s closed until fully open again
        }

        public void ForceHide()
        {
            TweenOut();
        }
    }
}