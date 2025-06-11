using UnityEngine;
using System.Collections.Generic;
using System;
using Team.Data;
using Team.Gameplay.GridSystem;
using System.Linq;
using Team.Gameplay.TurnSystem;
using Team.UI.Gameplay;
using Team.Gameplay.Characters;

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
        private List<CharacterReskinData> _characterReskinList = new List<CharacterReskinData>();

        [SerializeField]
        private Dictionary<CharacterData, Base_Ch> CharactersInLevel = new Dictionary<CharacterData, Base_Ch>();

        [SerializeField]
        private Dictionary<CharacterColorCode, CharacterReskinData> _characterReskinMap = new Dictionary<CharacterColorCode, CharacterReskinData>();

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
            LoadCharacterReskinMap();
            SpawnAllCharacters();
        }

        #endregion

        #region Public Methods

        public Base_Ch GetCharacter(string _characterName)
        {
            var characterObject = CharactersInLevel.First(x => x.Key.CharacterID == _characterName).Value;

            return characterObject;
        } 

        public void SpawnAllCharacters()
        {
            foreach (var character in CharactersMap)
            {
                Debug.Log($"Loading character: {character.CharacterID}");
                AddCharacter(character);
            }

            GameTurnManager.Instance.OnCharactersLoaded();
        }

        public void AddCharacter(CharacterData data)
        {
            //Spawn the character
            var characterObject = Instantiate(data.CharacterPrefab);

            TileID tileID = new TileID((int)data.StartTileID.x, (int)data.StartTileID.y);

            var baseCharacterRef = characterObject.GetComponent<Base_Ch>();
            baseCharacterRef.InitialiseCharacter(tileID, data.FacingDirection);

            //Reskin character if reskinner exists
            if(characterObject.TryGetComponent<CharacterReskinner>(out var characterReskinner))
            {
                characterReskinner.SetCharacterReskin(data.CharacterSkin);
            }

            LoadCardUI(baseCharacterRef, data);

            CharactersInLevel.Add(data, baseCharacterRef);
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
            var characterSkinner = _character.GetComponent<CharacterReskinner>();
            cardUI.PopulateUICardData(data, characterSkinner);

            GameTurnManager.Instance.AddCharacterToTurnOrder(gameCard);
        }

        private void LoadCharacterReskinMap()
        {
            foreach(var _characterSkin in _characterReskinList)
            {
                if (!_characterReskinMap.ContainsKey(_characterSkin.CharacterCode))
                {
                    _characterReskinMap.Add(_characterSkin.CharacterCode, _characterSkin);
                }
            }
        }

        #endregion
    }
}
