using Team.Data;
using Team.Gameplay.Characters;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Team.UI.Gameplay
{
    public class UIGameCard : UIDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image cardImage;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private CharacterReskinner characterReskinner;

        [SerializeField]
        private Base_Ch characterRef;

        [SerializeField]
        private Vector3 defaultScale = Vector3.one;

        [SerializeField]
        private Vector3 selectedScale = new Vector3(1.25f, 1.25f, 1.25f);

        [SerializeField]
        private TextMeshProUGUI turnIndexNumber; //This will be turned to image later in future

        [Space(5)]
        [Header("Card Components")]

        [SerializeField]
        private Color interactableColor;

        [SerializeField]
        private Color unInteractableColor;

        #region Unity Methods
        protected void Start()
        {
            _turnHolder.OnTurnOrderUpdatedEvent += UpdateTurnIndexText;
            OnSiblingIndexUpdatedEvent += OnSiblingIndexUpdatedEventHandler;

            UpdateTurnIndexText();
        }

        private void OnDestroy()
        {
            OnSiblingIndexUpdatedEvent -= OnSiblingIndexUpdatedEventHandler;
            _turnHolder.OnTurnOrderUpdatedEvent -= UpdateTurnIndexText;
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
            turnIndexNumber.text = transform.GetSiblingIndex().ToString();
        }

        #endregion

        #region Public Methods
        public void PopulateUICardData(CharacterData data, CharacterReskinner _skinner)
        {
            gameObject.name = $"Game-Card: {data.CharacterID}";
            interactableColor = data.CharacterSkin.CharacterColor;
            cardImage.color = interactableColor;

            nameText.text = data.CharacterID;

            characterReskinner = _skinner;

            characterRef = characterReskinner.gameObject.GetComponent<Base_Ch>();

            var uiCharacter = characterReskinner.UICharacter;
            uiCharacter?.PopulateCharacterUI(data.CharacterID, data.CharacterSkin);

            MakeInteractable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_turnHolder.HasSelected) return;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            //Have base character toggle Ghosting (characterRef)
            characterRef.ToggleGhosting();

            //Fire On Click Bark
            characterRef.OnClickBark();
        }

        [ContextMenu("Make Interactable")]
        public void MakeInteractable()
        {
            canvasGroup.interactable = true;
            cardImage.raycastTarget = true;
            cardImage.color = interactableColor;
            
        }

        [ContextMenu("Make Uninteractable")]
        public void MakeUninteractable()
        {
            canvasGroup.interactable = false;
            cardImage.raycastTarget = false;
            cardImage.color = unInteractableColor;
        }

        #endregion

        #region Drag Handlers Section

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            _turnHolder.ActivateGhostCard();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            _turnHolder.DeactivateGhostCard();
        }

        #endregion
    }
}
