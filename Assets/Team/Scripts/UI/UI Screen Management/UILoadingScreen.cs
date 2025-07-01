using Team.Managers;
using UnityEngine;

namespace Team.UI
{
    public class UILoadingScreen : UIScreen
    {
        [SerializeField]
        private GameLoadManager gameLoadManager;

        [SerializeField]
        private float value;

        public override void Start()
        {
            base.Start();

            gameLoadManager.OnLoadPercentChangedEvent += OnLoadPercentChangedEventHandler;
        }

        private void OnDestroy()
        {
            gameLoadManager.OnLoadPercentChangedEvent -= OnLoadPercentChangedEventHandler;
        }

        private void OnLoadPercentChangedEventHandler(float percent)
        {
            value = percent;    
        }
    }
}
