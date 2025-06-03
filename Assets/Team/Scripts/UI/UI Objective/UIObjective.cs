using TMPro;
using UnityEngine;

namespace Team.UI
{
    public class UIObjective : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI objectiveText;

        [SerializeField]
        private string objectiveTitle;


        public void Populate(string _message)
        {
            objectiveTitle = _message;
            objectiveText.text = _message;
            InComplete();
        }

        public void Toogle(bool _complete)
        {
            if (_complete)
            {
                Completed();
            }
            else
            {
                InComplete();
            }
        }

        private void Completed()
        {
            objectiveText.text = $"<s>{objectiveTitle}</s>";
        }

        public void InComplete()
        {
            objectiveText.text = $"{objectiveTitle}";
        }
    }
}
