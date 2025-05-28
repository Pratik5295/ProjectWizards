namespace Team.UI.Gameplay
{
    public class UIGameCard : UIDragHandler
    {
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
    }
}

