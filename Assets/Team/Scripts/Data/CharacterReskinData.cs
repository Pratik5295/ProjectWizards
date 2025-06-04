using UnityEngine;

namespace Team.Data
{
    public enum CharacterColorCode
    {
        RED = 0,
        BLUE = 1,
        BLACK = 3
    }

    [System.Serializable]
    public class CharacterReskinData
    {
        public CharacterColorCode CharacterCode;

        public Material SkinMaterial;

        [Tooltip("Will be used on UI Card, Outline Shader and anywhere required")]
        public Color CharacterColor;
    }
}