using Team.Gameplay.GridSystem;
using UnityEngine;

namespace Team.Data
{
    [System.Serializable]
    public class CharacterData
    {
        public GameObject CharacterPrefab;  //Prefab to be loaded
        public TileID StartTileID;
        public GameObject UICardPrefab;
    }
}
