using UnityEngine;
using System.Collections.Generic;
using System;
using Team.Data;
using Team.Gameplay.GridSystem;
using System.Linq;

namespace Team.Managers
{

    public class CharacterManager : MonoBehaviour
    {
        
        public static CharacterManager Instance = null;


        [Tooltip("Load all the characters that would be spawned")]
        [SerializeField]
        private List<CharacterData> CharactersMap = new List<CharacterData>(); 

        [SerializeField]
        private Dictionary<Guid,Base_Ch> CharactersInLevel = new Dictionary<Guid,Base_Ch>();


        #region Unity Methods
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SpawnAllCharacters();
        }

        #endregion

        #region Public Methods

        public void SpawnAllCharacters()
        {
            foreach (var character in CharactersMap)
            {
                AddCharacter(character);
            }
        }

        public void AddCharacter(CharacterData data)
        {
            //Create a key
            Guid characterKey = Guid.NewGuid();

            //Spawn the character
            var characterObject = Instantiate(data.CharacterPrefab);

            TileID tileID = new TileID((int)data.StartTileID.x, (int)data.StartTileID.y);
            var tile = GridManager.Instance.FindTile(tileID);

            characterObject.transform.position = new Vector3(tile.TilePosition.x, tile.TilePosition.y + 1f, tile.TilePosition.z);

            var baseCharacterRef = characterObject.GetComponent<Base_Ch>();
            //TODO:Set current tile reference through here

            CharactersInLevel.Add(characterKey, baseCharacterRef);
        }

        public void RemoveCharacter(Base_Ch _character)
        {
           var kvp = CharactersInLevel.First(x => x.Value == _character);
           CharactersInLevel.Remove(kvp.Key);

           Destroy(kvp.Value.gameObject);
        }

        [ContextMenu("Reset all characters")]
        public void ResetAllCharacters()
        {
            foreach(var _character in CharactersInLevel)
            {
                Destroy(_character.Value.gameObject);
            }

            CharactersInLevel.Clear();
        }
        #endregion
    }
}
