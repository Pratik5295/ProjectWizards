using System.Collections.Generic;
using DG.Tweening;
using Team.Data;
using Team.GameConstants;
using Team.Gameplay.Characters;
using Team.Managers;
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

        private ObjectClickable objectClickable;

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
            OnSiblingIndexUpdatedEvent += OnSiblingIndexUpdatedEventHandler;

            UpdateTurnIndexText();
        }

        private void OnDestroy()
        {
            OnSiblingIndexUpdatedEvent -= OnSiblingIndexUpdatedEventHandler;

            if (objectClickable)
            {
                objectClickable.onHovered.RemoveAllListeners();
                objectClickable.OnEnableClick.RemoveAllListeners();
            }
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

        public void UpdateCharacterNumber(int _number)
        {
            characterRef.UpdateCharacterNumber(_number);
        }

        #endregion

        #region Public Methods
        public void PopulateUICardData(CharacterData data, CharacterReskinner _skinner)
        {
            gameObject.name = $"Game-Card: {data.CharacterID}";

            var CharacterCode = data.CharacterSkin.CharacterCode;

            wizardImage.sprite = cardSprites[(int)CharacterCode];

            characterReskinner = _skinner;

            characterRef = characterReskinner.gameObject.GetComponent<Base_Ch>();

            objectClickable = characterRef.GetComponent<ObjectClickable>();
            objectClickable.onHovered.AddListener(OnObjectHovered);
            objectClickable.OnEnableClick.AddListener(OnDetectedClick);

            var uiCharacter = characterReskinner.UICharacter;
            uiCharacter?.PopulateCharacterUI(data.CharacterID, data.CharacterSkin);

            selectedOutline.SetActive(false);

            MakeInteractable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_turnHolder.HasSelected) return;

            if (characterRef.GetGhostingLock)
            {
                characterRef.SetGhosting(true);
            }

            HighlightCard();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (characterRef.GetGhostingLock)
            {
                characterRef.SetGhosting(false);
            }

            UnhighlightCard();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ClickedOnCharacter();
        }

        [ContextMenu("Make Interactable")]
        public void MakeInteractable()
        {
            if (canvasGroup == null) return;

            canvasGroup.interactable = true;
            turnCompletedCard.gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = true;

        }

        [ContextMenu("Make Uninteractable")]
        public void MakeUninteractable()
        {
            if (canvasGroup == null) return;

            canvasGroup.interactable = false;
            turnCompletedCard.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = false;
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


        #region Highlight Identifiers Section

        public void HighlightCard()
        {
            selectedOutline.SetActive(true);

            transform.DOScale(selectedScale, MetaConstants.ScaleMaxTimer).SetEase(Ease.OutBack).WaitForCompletion();
        }

        public void UnhighlightCard()
        {
            selectedOutline.SetActive(false);

            transform.DOScale(defaultScale, MetaConstants.ScaleMaxTimer).SetEase(Ease.OutBack).WaitForCompletion();
        }

        private void OnObjectHovered(bool _hovered)
        {
            if (_hovered)
            {
                HighlightCard();
            }
            else
            {
                UnhighlightCard();
            }
        }

        private void OnDetectedClick()
        {
            ClickedOnCharacter();
            UnhighlightCard();

            characterRef.RefreshGhosting(); 
        }

        private void ClickedOnCharacter()
        {
            //Toggle Lock Ghosting Effect
            characterRef.ToggleGhostingLock();
            //Fire On Click Bark
            characterRef.OnClickBark();
        }

        #endregion
    }
}
