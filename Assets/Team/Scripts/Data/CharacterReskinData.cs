using UnityEngine;

namespace Team.Data
{
    public enum CharacterColorCode
    {
        FIRE = 0,
        PUSH = 1,
        REDIRECT = 2,
        ROTATE = 3
    }

    [System.Serializable]
    public class CharacterReskinData
    {
        //TO BE REMOVED?
        public CharacterColorCode CharacterCode;

        public Material SkinMaterial;

        [Tooltip("Will be used on UI Card, Outline Shader and anywhere required")]
        public Color CharacterColor;
    }
}