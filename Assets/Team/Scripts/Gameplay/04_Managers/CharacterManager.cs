using UnityEngine;
using System.Collections.Generic;
using System;
using Team.Data;
using Team.Gameplay.GridSystem;
using System.Linq;
using Team.Gameplay.TurnSystem;
using Team.UI.Gameplay;

namespace Team.Managers
{
    [DefaultExecutionOrder(2)]
    public class CharacterManager : MonoBehaviour
    {
        
        public static CharacterManager Instance = null;


        [Tooltip("Load all the characters that would be spawned")]
        [SerializeField]
        private List<CharacterData> CharactersMap = new List<CharacterData>(); 

        [SerializeField]
        private Dictionary<Guid,Base_Ch> CharactersInLevel = new Dictionary<Guid,Base_Ch>();

        [SerializeField]
        private Transform cardHolder;


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

        public async void SpawnAllCharacters()
        {
            foreach (var character in CharactersMap)
            {
                AddCharacter(character);
            }

            //All characters are spawned. Have the turn order set the current order

            await GameTurnManager.Instance.LoadQueue();
        }

        public void AddCharacter(CharacterData data)
        {
            //Create a key
            Guid characterKey = Guid.NewGuid();

            //Spawn the character
            var characterObject = Instantiate(data.CharacterPrefab);

            TileID tileID = new TileID((int)data.StartTileID.x, (int)data.StartTileID.y);

            var baseCharacterRef = characterObject.GetComponent<Base_Ch>();
            baseCharacterRef.InitialiseCharacter(tileID);

            LoadCardUI(baseCharacterRef, data);

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
            //Delete all characters
            foreach(var _character in CharactersInLevel)
            {
                Destroy(_character.Value.gameObject);
            }

            CharactersInLevel.Clear();

            //Delete all Game Cards
            for(int i = 0; i < cardHolder.childCount; i++)
            {
                var card = cardHolder.GetChild(i);
                Destroy(card.gameObject);
            }
        }
        #endregion

        #region Private Methods

        private void LoadCardUI(Base_Ch _character, CharacterData data)
        {
            var gameCard = Instantiate(data.UICardPrefab, cardHolder);
            var gameTurn = gameCard.GetComponent<GameTurn>();
            gameTurn.SetupGameTurn(_character);

            var cardUI = gameCard.GetComponent<UIGameCard>();
            cardUI.PopulateUICardData(data);

            GameTurnManager.Instance.AddCharacterToTurnOrder(gameCard);
        }

        #endregion
    }
}
