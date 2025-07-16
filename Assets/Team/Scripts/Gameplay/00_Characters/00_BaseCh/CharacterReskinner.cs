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
        public const float GlowIntensity = 1.25f;
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

        public Color DefaultObjectiveColor;
        public Color PrimaryObjectiveColor;
        public Color SecondaryObjectiveColor;


        [SerializeField]
        private SimpleOutline outlineComponent; //Reference to the outline shader attached on body

        public void SetupCharacterOutline(CharacterReskinData _reskinData)
        {
            if (outlineComponent != null)
            {
                outlineComponent.OutlineWidth = MetaConstants.CharacterOutlineThickness;


                GlowUp();
            }

            HideOutline();
        }


        public void ShowOutline()
        {
            if (IsObjective)
            {
                outlineComponent.OutlineColor = DefaultObjectiveColor;

                GlowUp();
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
                switch (_cachedPriority)
                {
                    case ObjectivePriority.PRIMARY:
                        outlineComponent.OutlineColor = PrimaryObjectiveColor;
                        break;
                    case ObjectivePriority.SECONDARY:
                        outlineComponent.OutlineColor = SecondaryObjectiveColor;
                        break;
                }

                GlowUp();

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

            switch (priority)
            {
                case ObjectivePriority.PRIMARY:
                    outlineComponent.OutlineColor = PrimaryObjectiveColor;
                    break;
                case ObjectivePriority.SECONDARY:
                    outlineComponent.OutlineColor = SecondaryObjectiveColor;
                    break;
            }
            

            IsObjective = true;

            //Force enabled for objectives
            outlineComponent.enabled = true;

            GlowUp();

        }

        private void GlowUp()
        {
            Color hdrColor = outlineComponent.OutlineColor * MetaConstants.GlowIntensity;
            // Ensure alpha stays the same
            hdrColor.a = outlineComponent.OutlineColor.a;
            outlineComponent.OutlineColor = hdrColor;
        }
    }
}
