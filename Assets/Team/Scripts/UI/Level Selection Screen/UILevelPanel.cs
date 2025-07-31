using TMPro;
using UnityEngine;

namespace Team.UI
{

    public class UILevelPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI levelTitle;

        [SerializeField]
        private TextMeshProUGUI levelDescription;

        [SerializeField]
        private TextMeshProUGUI levelObjective;


        public void PopulateData(string _title,string _description)
        {
            levelTitle.text = _title;
            levelDescription.text = _description;
        }
    }
}