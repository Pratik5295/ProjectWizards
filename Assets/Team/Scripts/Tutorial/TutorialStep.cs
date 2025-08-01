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
            BOX_5,
            R_BoardBox,
            L_BoardBox,
            TurnOrderBox,
            ObjectiveBox,
            PlayBox,
            ExplosiveBox

        }
    }
}

namespace Team.Gameplay.Tutorial
{
    [System.Serializable]
    public class TutorialData
    {
        public string header;
        public string message;
        public TutUIElement uiElement;

        public TutorialData(TutorialData _data)
        {
            header = _data.header;
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
        private TextMeshProUGUI _headerText;
        [SerializeField]
        private TextMeshProUGUI _tutorialText;

        public void PopulateTutorialBox(string _header, string _message)
        {
            _headerText.text = _header;
            _tutorialText.text = _message;  
        }

        public void OnTutorialBoxClicked()
        {
            //Update the tutorial manager
            TutorialManager.Instance.GoToNextStep();
        }
    }
}
