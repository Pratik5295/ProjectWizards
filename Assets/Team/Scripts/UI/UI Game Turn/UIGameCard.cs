using Team.Data;
using Team.Gameplay.Characters;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Team.UI.Gameplay
{
    public class UIGameCard : UIDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Image cardImage;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private CharacterReskinner characterReskinner;

        [SerializeField]
        private Vector3 defaultScale = Vector3.one;

        [SerializeField]
        private Vector3 selectedScale = new Vector3(1.25f, 1.25f, 1.25f);

        #region Unity Methods
        protected void Start()
        {
            OnSiblingIndexUpdatedEvent += OnSiblingIndexUpdatedEventHandler;
        }

        private void OnDestroy()
        {
            OnSiblingIndexUpdatedEvent -= OnSiblingIndexUpdatedEventHandler;
        }
        #endregion

        #region Event Listeners
        private void OnSiblingIndexUpdatedEventHandler(int _newIndex)
        {
            // Notify turn manager of updated order
            GameTurnManager.Instance.ForceRebuildTurns();
        }
        #endregion

        #region Public Methods
        public void PopulateUICardData(CharacterData data, CharacterReskinner _skinner)
        {
            gameObject.name = $"Game-Card: {data.CharacterID}";
            cardImage.color = data.CharacterSkin.CharacterColor;

            nameText.text = data.CharacterID;

            characterReskinner = _skinner;

            var uiCharacter = characterReskinner.UICharacter;
            uiCharacter?.PopulateCharacterUI(data.CharacterID, data.CharacterSkin);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            characterReskinner.ShowOutline();

            transform.localScale = selectedScale;

            LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent as RectTransform);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            characterReskinner.HideOutline();

            transform.localScale = defaultScale;

            LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent as RectTransform);
        }
        #endregion
    }
}
