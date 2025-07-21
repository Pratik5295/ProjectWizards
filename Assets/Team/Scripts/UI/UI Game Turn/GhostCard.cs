using Team.UI.Gameplay;
using UnityEngine;

namespace Team.UI
{
    public class GhostCard : MonoBehaviour
    {
        [SerializeField]
        private UIGameCard pairedCard;

        public void PairCardWith(UIGameCard card)
        {
            pairedCard = card;
        }

        public void ResetGhostCard()
        {
            PairCardWith(null);
        }
    }
}
