using TMPro;
using UnityEngine;
using System.Collections.Generic;

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


        public void PopulateData(string _title,string _description, List<string> _objectives)
        {
            levelTitle.text = _title;
            levelDescription.text = _description;

            string message = string.Empty;
            foreach (var objective in _objectives)
            {
                message += objective + " ,";
            }

            levelObjective.text = message;
        }
    }
}