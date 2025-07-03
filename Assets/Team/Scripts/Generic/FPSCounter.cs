using TMPro;
using UnityEngine;

namespace Team.Generic
{
    public class FPSCounter : MonoBehaviour
    {
        public TextMeshProUGUI fpsText;
        public float updateInterval = 0.5f;

        private float timeSinceLastUpdate = 0f;
        private int frames = 0;

        void Update()
        {
            frames++;
            timeSinceLastUpdate += Time.unscaledDeltaTime;

            if (timeSinceLastUpdate >= updateInterval)
            {
                float fps = frames / timeSinceLastUpdate;
                fpsText.text = $"FPS: {fps:F1}";

                frames = 0;
                timeSinceLastUpdate = 0f;
            }
        }
    }
}
