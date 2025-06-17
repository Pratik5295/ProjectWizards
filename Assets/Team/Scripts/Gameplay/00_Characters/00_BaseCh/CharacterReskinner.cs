using Team.Data;
using Team.GameConstants;
using UnityEngine;
namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float CharacterOutlineThickness = 0.01f;
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

        [SerializeField]
        private Renderer[] _bodyMeshes;    //Convert it to a list if the model is going to have multiple meshes to color

        [SerializeField]
        private Material outlineMat;

        private Material outlineInstMat;

        [SerializeField]
        private Material[] materialArray;
        public void SetCharacterReskin(CharacterReskinData _reskinData)
        {
            outlineInstMat = new Material(outlineMat);
            var bodyMat = _bodyMeshes[0].material;
            materialArray = new Material[2];

            materialArray[0] = _reskinData.SkinMaterial;
            materialArray[1] = outlineInstMat;

            for (int i = 0; i < _bodyMeshes.Length; i++)
            {
                _bodyMeshes[i].materials = materialArray;
            }

            HideOutline();
        }

        public void ShowOutline()
        {
            float thickness = MetaConstants.CharacterOutlineThickness;
            outlineInstMat.SetFloat("_Outline_Thickness", thickness);
        }

        public void HideOutline()
        {
            outlineInstMat.SetFloat("_Outline_Thickness", 0f);
        }
    }
}
