using System.Collections.Generic;
using System.Linq;
using Team.Data;
using Team.Gameplay.Characters;
using Team.Gameplay.GridSystem;
using Team.Gameplay.TurnSystem;
using Team.UI.Gameplay;
using UnityEngine;

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

        [SerializeField]
        private GameObject UIGameCardPrefab;

        [SerializeField]
        private bool toggleCharactersGhosting = false;


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
           
        }

        #endregion

        #region Public Methods

        public void LoadCharactersFromLeveData(List<CharacterData> _characters)
        {
            CleanUp();

            CharactersMap.Clear();

            //Load all the characters
            foreach (CharacterData character in _characters)
            {
                CharactersMap.Add(character);
            }

            //Initialize character dictionary & spawn characters
            SpawnAllCharacters();

        }

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

            GameTurnManager.Instance.OnTurnsProcessingEvent += TurnGhostingOff;
        }

        public void AddCharacter(CharacterData data)
        {
            //Spawn the character
            var characterObject = Instantiate(data.CharacterPrefab);
            characterObject.name = $"{data.CharacterID}";

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
           
           GameTurnManager.Instance.OnTurnsProcessingEvent -= TurnGhostingOff;

        }

        [ContextMenu("Remove all characters")]
        public void RemoveAllCharacters()
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

        public void ResetAllCharacters()
        {
            foreach (var _character in CharactersInLevel)
            {
                _character.Value.UndoMovement();
            }
        }

        public void CleanUp()
        {
            if (CharactersInLevel.Count == 0) return;

            RemoveAllCharacters();
        }

        public void ToggleGhosting()
        {
            toggleCharactersGhosting = !toggleCharactersGhosting;

            foreach (var _character in CharactersInLevel.Values)
            {
                //Toggle Ghosting here through a bool value
                _character.SetGhosting(toggleCharactersGhosting);
            }
        }

        public void TurnGhostingOff()
        {
            foreach(var character in CharactersInLevel.Values)
            {
                //Turn Ghosting off here
                character.SetGhosting(false);
            }
            toggleCharactersGhosting = false;
        }

        #endregion

        #region Private Methods

        private void LoadCardUI(Base_Ch _character, CharacterData data)
        {
            var gameCard = Instantiate(UIGameCardPrefab, cardHolder);
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
