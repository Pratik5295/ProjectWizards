using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Team.Data;
using Team.Gameplay.Characters;
using Team.Gameplay.GridSystem;
using Team.Gameplay.TurnSystem;
using Team.UI.Gameplay;
using UnityEngine;

namespace Team.Managers
{
    [DefaultExecutionOrder(2)]
    public class CharacterManager : MonoBehaviour, ILoadingOperation
    {
        public static CharacterManager Instance = null;

        [Tooltip("Load all the characters that would be spawned")]
        [SerializeField] private List<CharacterData> CharactersMap = new List<CharacterData>();
        [SerializeField] private List<CharacterReskinData> _characterReskinList = new List<CharacterReskinData>();
        [SerializeField] private Dictionary<CharacterData, Base_Ch> CharactersInLevel = new Dictionary<CharacterData, Base_Ch>();
        [SerializeField] private Dictionary<CharacterColorCode, CharacterReskinData> _characterReskinMap = new Dictionary<CharacterColorCode, CharacterReskinData>();
        [SerializeField] private Transform cardHolder;
        [SerializeField] private GameObject UIGameCardPrefab;
        [SerializeField] private bool toggleCharactersGhosting = false;

        public string Description => "Loading Characters...";

        private void Awake()
        {
            if (Instance == null)
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

        // Original synchronous method for backwards compatibility
        public void LoadCharactersFromLeveData(List<CharacterData> _characters)
        {
            LoadCharactersFromLevelDataAsync(_characters).Forget();
        }

        // New async method
        public async UniTask LoadCharactersFromLevelDataAsync(List<CharacterData> _characters, IProgress<float> progress = null)
        {
            try
            {
                Debug.Log("[CharacterManager] Starting character loading...");

                CleanUp();
                CharactersMap.Clear();

                // Copy character data (10% progress)
                progress?.Report(0.1f);
                foreach (CharacterData character in _characters)
                {
                    CharactersMap.Add(character);
                }

                // Spawn all characters with progress tracking (90% progress)
                await SpawnAllCharactersAsync(new Progress<float>(p => {
                    float currentProgress = 0.1f + (p * 0.9f);
                    progress?.Report(currentProgress);
                }));

                Debug.Log("[CharacterManager] Character loading completed!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CharacterManager] Error loading characters: {ex.Message}");
                throw;
            }
        }

        // Original synchronous method for backwards compatibility
        public void SpawnAllCharacters()
        {
            SpawnAllCharactersAsync().Forget();
        }

        // New async method
        public async UniTask SpawnAllCharactersAsync(IProgress<float> progress = null)
        {
            int totalCharacters = CharactersMap.Count;
            Debug.Log($"[CharacterManager] Spawning {totalCharacters} characters...");

            for (int i = 0; i < totalCharacters; i++)
            {
                var character = CharactersMap[i];
                Debug.Log($"[CharacterManager] Loading character: {character.CharacterID} ({i + 1}/{totalCharacters})");

                await AddCharacterAsync(character);

                // Report progress
                float characterProgress = (float)(i + 1) / totalCharacters;
                progress?.Report(characterProgress);

                // Yield control to prevent frame drops
                await UniTask.Yield();
            }

            Debug.Log("[CharacterManager] All characters spawned, notifying GameTurnManager...");
            GameTurnManager.Instance.OnCharactersLoaded();
            GameTurnManager.Instance.OnTurnsProcessingEvent += TurnGhostingOff;
            Debug.Log("[CharacterManager] Character spawning complete!");
        }

        // Original synchronous method for backwards compatibility
        public void AddCharacter(CharacterData data)
        {
            AddCharacterAsync(data).Forget();
        }

        // New async method
        public async UniTask AddCharacterAsync(CharacterData data)
        {
            // Spawn the character
            var characterObject = Instantiate(data.CharacterPrefab);
            characterObject.name = $"{data.CharacterID}";

            TileID tileID = new TileID((int)data.StartTileID.x, (int)data.StartTileID.y);

            var baseCharacterRef = characterObject.GetComponent<Base_Ch>();
            baseCharacterRef.InitialiseCharacter(tileID, data.FacingDirection);

            // Reskin character if reskinner exists
            if (characterObject.TryGetComponent<CharacterReskinner>(out var characterReskinner))
            {
                characterReskinner.SetupCharacterOutline(data.CharacterSkin);
            }

            await LoadCardUIAsync(baseCharacterRef, data);

            CharactersInLevel.Add(data, baseCharacterRef);
        }

        // ILoadingOperation implementation
        public async UniTask LoadAsync(IProgress<float> progress = null)
        {
            await LoadCharactersFromLevelDataAsync(CharactersMap, progress);
        }

        public Base_Ch GetCharacter(string _characterName)
        {
            var characterObject = CharactersInLevel.First(x => x.Key.CharacterID == _characterName).Value;
            return characterObject;
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
            foreach (var _character in CharactersInLevel)
            {
                Destroy(_character.Value.gameObject);
            }

            CharactersInLevel.Clear();

            for (int i = 0; i < cardHolder.childCount; i++)
            {
                var card = cardHolder.GetChild(i);
                if (card.gameObject.GetComponent<GameBreakpoint>() == null)
                {
                    Destroy(card.gameObject);
                }
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
                _character.SetGhosting(toggleCharactersGhosting);
            }
        }

        public void TurnGhostingOff()
        {
            foreach (var character in CharactersInLevel.Values)
            {
                character.SetGhosting(false);
            }
            toggleCharactersGhosting = false;
        }

        private async UniTask LoadCardUIAsync(Base_Ch _character, CharacterData data)
        {
            var gameCard = Instantiate(UIGameCardPrefab, cardHolder);
            var gameTurn = gameCard.GetComponent<GameTurn>();
            gameTurn.SetupGameTurn(_character);

            var cardUI = gameCard.GetComponent<UIGameCard>();
            var characterSkinner = _character.GetComponent<CharacterReskinner>();
            cardUI.PopulateUICardData(data, characterSkinner);

            GameTurnManager.Instance.AddCharacterToTurnOrder(gameCard);

            // Allow UI to update
            await UniTask.Yield();
        }

        private void LoadCharacterReskinMap()
        {
            foreach (var _characterSkin in _characterReskinList)
            {
                if (!_characterReskinMap.ContainsKey(_characterSkin.CharacterCode))
                {
                    _characterReskinMap.Add(_characterSkin.CharacterCode, _characterSkin);
                }
            }
        }
    }
}
