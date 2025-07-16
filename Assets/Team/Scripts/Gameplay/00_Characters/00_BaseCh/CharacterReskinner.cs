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

        public bool isOutlined { get; private set; }

        private bool IsObjective = false;
        public Color PrimaryObjectiveColor;
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
            isOutlined = true;

            if (outlineComponent != null)
            {
                outlineComponent.enabled = true;
            }
        }

        public void HideOutline()
        {
            if (IsObjective) return;

            isOutlined = false;

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

            outlineComponent.OutlineColor = PrimaryObjectiveColor;

            IsObjective = true;

            ShowOutline();
        }
    }
}
