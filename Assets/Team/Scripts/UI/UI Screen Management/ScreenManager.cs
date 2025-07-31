using System.Collections.Generic;
using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public enum GameScreen
        {
            EMPTY = 0,
            MENU = 1,
            DIALOGUE = 2,
            GAME = 3,
            CHAPTER_SELECT = 4,
            POST_GAME = 5,
            LOADING = 6,
            LEVEL_SELECT = 7
        }
    }
}


namespace Team.UI
{

    public class ScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private List<UIScreen> screens;
        [SerializeField]
        private UIScreen currentScreen;

        public UIScreen CurrentScreen => currentScreen;

        private Dictionary<GameScreen, UIScreen> screenMap = new();

        [SerializeField]
        private UIGameScreen gameScreen;

        public UIGameScreen GameScreen => gameScreen;

        private void Awake()
        {
            foreach (var screen in screens)
            {
                if (!screenMap.ContainsKey(screen.screenType))
                {
                    screenMap.Add(screen.screenType, screen);
                    screen.OnHide(); // Hide all initially
                }
                else
                {
                    Debug.LogWarning($"Duplicate screen type: {screen.screenType}");
                }
            }
        }

        public void ShowScreen(GameScreen screenType)
        {
            if (currentScreen != null)
            {
                currentScreen.OnHide();
            }

            if (screenMap.TryGetValue(screenType, out var newScreen))
            {
                newScreen.OnShow();
                currentScreen = newScreen;
            }
            else
            {
                Debug.LogError($"Screen not found: {screenType}");
            }
        }

        public UIScreen GetScreen(GameScreen screenType)
        {
            return screenMap.TryGetValue(screenType, out var screen) ? screen : null;
        }
    }
}
