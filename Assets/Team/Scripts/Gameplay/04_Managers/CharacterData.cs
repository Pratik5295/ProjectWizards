using Team.Gameplay.GridSystem;
using UnityEngine;

namespace Team.Data
{
    [System.Serializable]
    public class CharacterData
    {
        public string CharacterID; //String value to represent each character
        public GameObject CharacterPrefab;  //Prefab to be loaded
        public TileID StartTileID;
        public GameObject UICardPrefab;

        //Reskin Details
        public CharacterReskinData CharacterSkin;
    }
}
