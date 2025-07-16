using Team.Data;
using Team.GameConstants;
using Team.Gameplay.ObjectiveSystem;
using Team.UI;
using UnityEngine;
namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float CharacterOutlineThickness = 4f;
    }
}

namespace Team.Gameplay.Characters
{
    [RequireComponent(typeof(Base_Ch))]
    public class CharacterReskinner : MonoBehaviour
    {
        [SerializeField]
        private UICharacter uiCharacter; //TODO: Move it out of here

        public UICharacter UICharacter => uiCharacter;

        private bool IsObjective = false;

        [SerializeField]
        private ObjectivePriority _cachedPriority;

        [ColorUsageAttribute(true, true)]
        public Color DefaultObjectiveColor;
        [ColorUsageAttribute(true, true)]
        public Color PrimaryObjectiveColor;
        [ColorUsageAttribute(true, true)]
        public Color SecondaryObjectiveColor;


        [SerializeField]
        private SimpleOutline outlineComponent; //Reference to the outline shader attached on body


        public void SetupCharacterOutline(CharacterReskinData _reskinData)
        {
            if (outlineComponent != null)
            {
                outlineComponent.OutlineWidth = MetaConstants.CharacterOutlineThickness;
            }

            HideOutline();
        }


        public void ShowOutline()
        {
            if (IsObjective)
            {
                SetOutlineColor(DefaultObjectiveColor);
            }

            if (outlineComponent != null)
            {
                outlineComponent.enabled = true;
            }
        }

        public void HideOutline()
        {
            if (IsObjective)
            {
                //Switch to based on priority
                SwitchColorOnPriority(_cachedPriority);

                return;
            }

            if (outlineComponent != null)
            {
                outlineComponent.enabled = false;
            }
        }

        public void SetTargetObjective(ObjectivePriority priority)
        {
            if (outlineComponent == null)
            {
                Debug.LogError($"Couldnt add priority shader as outline is missing for {gameObject.name}");
                return;
            }

            _cachedPriority = priority;

            SwitchColorOnPriority(priority);

             IsObjective = true;

            //Force enabled for objectives
            outlineComponent.enabled = true;

        }

        private void SetOutlineColor(Color color)
        {
            outlineComponent.OutlineColor = color;
        }

        private void SwitchColorOnPriority(ObjectivePriority priority)
        {
            switch (priority)
            {
                case ObjectivePriority.PRIMARY:
                    SetOutlineColor(PrimaryObjectiveColor);
                    break;
                case ObjectivePriority.SECONDARY:
                    SetOutlineColor(SecondaryObjectiveColor);
                    break;

                default:
                    SetOutlineColor(DefaultObjectiveColor);
                    break;
            }
        }
    }
}
