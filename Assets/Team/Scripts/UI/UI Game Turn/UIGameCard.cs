using System.Collections.Generic;
using DG.Tweening;
using Ink;
using Team.Data;
using Team.GameConstants;
using Team.Gameplay.Characters;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float ScaleMaxTimer = 0.1f;
    }
}

namespace Team.UI.Gameplay
{
    public class UIGameCard : UIDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {


        [SerializeField]
        private CharacterReskinner characterReskinner;

        [SerializeField]
        private Base_Ch characterRef;

        [SerializeField]
        private Vector3 defaultScale = Vector3.one;

        [SerializeField]
        private Vector3 selectedScale = new Vector3(1.25f, 1.25f, 1.25f);

        [SerializeField]
        private Image turnIndexIdentifier;

        [SerializeField]
        private Image wizardImage;

        [SerializeField]
        private GameObject selectedOutline; //Game Object containing an image with selected outline effect

        [SerializeField]
        private Image turnCompletedCard;    //Just a low opacity darken outline image

        [Space(5)]
        [Header("Card Components")]

        [SerializeField]
        private Color interactableColor;

        [SerializeField]
        private Color unInteractableColor;

        [Space(5)]
        [Header("Card Sprites")]

        [SerializeField]
        private List<Sprite> cardSprites;

        #region Unity Methods
        protected void Start()
        {
 /*           _turnHolder.OnTurnOrderUpdatedEvent += UpdateTurnIndexText;*/
            OnSiblingIndexUpdatedEvent += OnSiblingIndexUpdatedEventHandler;

            UpdateTurnIndexText();

            CharacterNumberHandler char_Number = characterRef.GetComponent<CharacterNumberHandler>();

            if (char_Number != null)
            {
                int index_Number = transform.GetSiblingIndex();
                char_Number.UpdateCharacterNumberText(index_Number);
            }
        }

        private void OnDestroy()
        {
            OnSiblingIndexUpdatedEvent -= OnSiblingIndexUpdatedEventHandler;
/*            _turnHolder.OnTurnOrderUpdatedEvent -= UpdateTurnIndexText;*/
        }


        #endregion

        #region Event Listeners
        private void OnSiblingIndexUpdatedEventHandler(int _newIndex)
        {
            // Notify turn manager of updated order
            GameTurnManager.Instance.ForceRebuildTurns();

        }

        private void UpdateTurnIndexText()
        {
            Sprite currentIndex = GameTurnManager.Instance.GetTurnIdentifierSprite(gameObject);
            turnIndexIdentifier.sprite = currentIndex;
        }

        #endregion

        #region Public Methods
        public void PopulateUICardData(CharacterData data, CharacterReskinner _skinner)
        {
            gameObject.name = $"Game-Card: {data.CharacterID}";

            var CharacterCode = data.CharacterSkin.CharacterCode;

            wizardImage.sprite = cardSprites[(int)CharacterCode];

            //interactableColor = data.CharacterSkin.CharacterColor;
            //cardImage.color = interactableColor;

            //nameText.text = data.CharacterID;

            characterReskinner = _skinner;

            characterRef = characterReskinner.gameObject.GetComponent<Base_Ch>();

            var uiCharacter = characterReskinner.UICharacter;
            uiCharacter?.PopulateCharacterUI(data.CharacterID, data.CharacterSkin);

            selectedOutline.SetActive(false);

            MakeInteractable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_turnHolder.HasSelected) return;

            //characterReskinner.ShowOutline();

            selectedOutline.SetActive(true);

            characterRef.SetGhosting(true);

            transform.DOScale(selectedScale, MetaConstants.ScaleMaxTimer).SetEase(Ease.OutBack).WaitForCompletion();

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //characterReskinner.HideOutline();

            selectedOutline.SetActive(false);

            characterRef.SetGhosting(false);

            transform.DOScale(defaultScale, MetaConstants.ScaleMaxTimer).SetEase(Ease.OutBack).WaitForCompletion();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Have base character toggle Ghosting (characterRef)
            //characterRef.ToggleGhosting();

            //Toggle Lock Ghosting Effect

            //Fire On Click Bark
            characterRef.OnClickBark();
        }

        [ContextMenu("Make Interactable")]
        public void MakeInteractable()
        {
            canvasGroup.interactable = true;
            turnCompletedCard.gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = true;
            //cardImage.color = interactableColor;

        }

        [ContextMenu("Make Uninteractable")]
        public void MakeUninteractable()
        {
            canvasGroup.interactable = false;
            turnCompletedCard.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = false;
            //cardImage.color = unInteractableColor;
        }

        #endregion

        #region Drag Handlers Section

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _turnHolder.ActivateGhostCard(this);

            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            //Move Ghost card along with it

            var ghostIndex = _turnHolder.GetIndex(rectTransform.localPosition.x);
            _turnHolder.SetGhostIndex(ghostIndex);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            _turnHolder.DeactivateGhostCard();
        }

        #endregion
    }
}
