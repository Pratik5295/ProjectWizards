using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public enum TutUIElement
        {
            BOX_1,
            BOX_2, 
            BOX_3, 
            BOX_4,
        }
    }
}

namespace Team.Gameplay.Tutorial
{
    [System.Serializable]
    public class TutorialData
    {
        public string message;
        public TutUIElement uiElement;
    }

    public class TutorialStep : MonoBehaviour
    {
        public TutorialData tutorialData;

        public void OnTutorialBoxClicked()
        {
            //Update the tutorial manager
        }
    }
}
