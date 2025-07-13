using Team.Managers;
using TMPro;
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

        public TutorialData(TutorialData _data)
        {
            message = _data.message;
            uiElement = _data.uiElement;
        }
    }

    /// <summary>
    /// This script is going on the UI of each element
    /// </summary>
    public class TutorialStep : MonoBehaviour
    {
        [SerializeField]
        private TutUIElement elementID; //The id that will be used to identify which UI element this is

        public TutUIElement ElementID => elementID;

        [SerializeField]
        private TextMeshProUGUI _tutorialText;

        public void PopulateTutorialBox(string _message)
        {
            _tutorialText.text = _message;  
        }

        public void OnTutorialBoxClicked()
        {
            //Update the tutorial manager
            Debug.Log($"Clicked on: {gameObject.name}");
            TutorialManager.Instance.GoToNextStep();
        }
    }
}
