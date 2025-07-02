using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Team.UI
{
    public class UILoadingScreen : UIScreen
    {
        [SerializeField]
        private GameLoadManager gameLoadManager;

        [SerializeField]
        private float value; //Will be removed

        [SerializeField]
        private Slider percentSlider;

        [SerializeField]
        private TextMeshProUGUI loadingStatusText;

        public override void Start()
        {
            base.Start();

            gameLoadManager.OnLoadPercentChangedEvent += OnLoadPercentChangedEventHandler;
            gameLoadManager.OnLoadingStartedEvent += OnLoadingStartedHandler;
            gameLoadManager.OnLoadingFinishedEvent += OnLoadingFinishedHandler;
        }

        private void OnDestroy()
        {
            gameLoadManager.OnLoadPercentChangedEvent -= OnLoadPercentChangedEventHandler;
            gameLoadManager.OnLoadingStartedEvent -= OnLoadingStartedHandler;
            gameLoadManager.OnLoadingFinishedEvent -= OnLoadingFinishedHandler;
        }

        private void OnLoadPercentChangedEventHandler(float percent,string _message)
        {
            value = percent;

            percentSlider.value = percent;

            loadingStatusText.text = _message;
        }

        private void OnLoadingStartedHandler()
        {
            UIManager.Instance.ShowLoadingScreen();
        }

        private void OnLoadingFinishedHandler()
        {
            LevelManager.Instance.StartLevel();
        }
    }
}
