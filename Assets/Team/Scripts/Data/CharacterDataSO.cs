using UnityEngine;
using UnityEngine.Video;

namespace Team.Data
{
    [System.Serializable]
    public class CharacterDataStruct
    {
        public string CharacterName;
        public string AbilityName;
        public string AbilityDescription;
        public VideoClip AbilityVideo;
    }

    [CreateAssetMenu(fileName = "CharacterDataSO",menuName = "Team/Data/Characters/Create Character File")]
    public class CharacterDataSO : ScriptableObject
    {
        public CharacterDataStruct Data;
    }
}
