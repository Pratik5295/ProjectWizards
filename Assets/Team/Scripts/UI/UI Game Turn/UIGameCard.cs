using Team.Data;
using Team.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Team.UI.Gameplay
{
    public class UIGameCard : UIDragHandler, IInputDetectable
    {
        [SerializeField]
        private Image cardImage;

        #region Unity Methods
        protected override void Start()
        {
            base.Start();
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
            //Force the turn manager to rebuild
            GameTurnManager.Instance.ForceRebuildTurns();
        }

        #endregion

        #region Public Methods

        public void PopulateUICardData(CharacterData data)
        {
            //Set Card Game Object data
            gameObject.name = $"Game-Card: {data.CharacterID}";
            cardImage.color = data.CharacterSkin.CharacterColor;
        }

        public void OnElementDragged()
        {
            Debug.Log($"Player input dragged on:{name}");
            OnDrag();
        }

        public void OnElementDown()
        {
            Debug.Log($"Player input down on:{name}");
            OnBeginDrag();
        }

        public void OnElementUp()
        {
            Debug.Log($"Player input up on:{name}");
            OnEndDrag();
        }

        #endregion
    }
}

