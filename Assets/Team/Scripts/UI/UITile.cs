using TMPro;
using UnityEngine;

namespace Team.Gameplay.GridSystem
{
    public class UITile : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI tileText;

        public void PopulateTileText(string _tileId)
        {
            tileText.text = _tileId;
        }
    }
}
