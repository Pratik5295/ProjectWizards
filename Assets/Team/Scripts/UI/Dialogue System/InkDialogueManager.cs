using Ink.Runtime;
using TMPro;
using UnityEngine;

namespace Team.UI.DialogueSystem
{
    public class InkDialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI dialogueText; // Assign in Inspector
        public TextAsset inkJSONAsset;
        private Story story;

        void Start()
        {
            story = new Story(inkJSONAsset.text);
            ContinueStory();
        }

        public void ContinueStory()
        {
            if (story.canContinue)
            {
                dialogueText.text = story.Continue(); // Gets next line
            }
            else
            {
                dialogueText.text = "The End.";
            }
        }

    }
}
