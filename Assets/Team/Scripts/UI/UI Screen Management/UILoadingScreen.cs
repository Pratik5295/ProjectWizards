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
        }

        private void OnDestroy()
        {
            gameLoadManager.OnLoadPercentChangedEvent -= OnLoadPercentChangedEventHandler;
        }

        private void OnLoadPercentChangedEventHandler(float percent,string _message)
        {
            value = percent;

            percentSlider.value = percent;

            loadingStatusText.text = _message;
        }
    }
}
