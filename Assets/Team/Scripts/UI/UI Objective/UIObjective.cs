using Team.Gameplay.ObjectiveSystem;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

namespace Team.UI
{
    public class UIObjective : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _objectiveTextBox;

        [SerializeField]
        private TextMeshProUGUI objectiveText;

        [SerializeField]
        private SVGImage iconImage;

        [SerializeField]
        private string objectiveTitle;

        [SerializeField]
        private Sprite primarySprite;
        [SerializeField]
        private Sprite secondarySprite;


        public void Populate(GameObjectiveData _data)
        {
            objectiveTitle = _data.ObjectiveName;
            objectiveText.text = objectiveTitle;

            UpdateIconBasedOnPriority(_data.Priority);

            InComplete();
        }

        public void Toogle(bool _complete)
        {
            if (_complete)
            {
                Completed();
            }
            else
            {
                InComplete();
            }
        }

        private void Completed()
        {
            objectiveText.text = $"<s>{objectiveTitle}</s>";
        }

        public void InComplete()
        {
            objectiveText.text = $"{objectiveTitle}";
        }

        private void UpdateIconBasedOnPriority(ObjectivePriority _priority)
        {
            switch (_priority)
            {
                case ObjectivePriority.PRIMARY:
                    //iconImage.sprite = primarySprite;
                    _objectiveTextBox.text = "Primary Objective:";
                    break;

                case ObjectivePriority.SECONDARY:
                    //iconImage.sprite = secondarySprite;
                    _objectiveTextBox.text = "Secondary Objective:";
                    break;

                default:
                    break;
            }
        }
    }
}
