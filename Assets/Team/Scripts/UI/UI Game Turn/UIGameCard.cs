using Team.Data;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Team.UI.Gameplay
{
    public class UIGameCard : UIDragHandler
    {
        [SerializeField]
        private Image cardImage;

        [SerializeField]
        private TextMeshProUGUI nameText;

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
        public void PopulateUICardData(CharacterData data)
        {
            gameObject.name = $"Game-Card: {data.CharacterID}";
            cardImage.color = data.CharacterSkin.CharacterColor;

            nameText.text = data.CharacterID;
        }
        #endregion
    }
}
