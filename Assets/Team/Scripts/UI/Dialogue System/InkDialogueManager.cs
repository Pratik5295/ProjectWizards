using Ink.Runtime;
using Team.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Team.UI.DialogueSystem
{
    public class InkDialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI dialogueText;
        public TextMeshProUGUI nameText;
        public Image portraitImage;
        private Story story;

        public Sprite flameWizard;
        public Sprite pushWizard;

        public void SetDialogue(TextAsset _asset)
        {
            story = new Story(_asset.text);
            ContinueStory();
        }

        public void ContinueStory()
        {
            if (story.canContinue)
            {
                string text = story.Continue().Trim();
                dialogueText.text = text;

                // Default values
                string speaker = "";
                string portraitTag = "";

                // Process tags
                foreach (var tag in story.currentTags)
                {
                    if (tag.StartsWith("speaker:"))
                        speaker = tag.Substring("speaker:".Length).Trim();

                    if (tag.StartsWith("portrait:"))
                        portraitTag = tag.Substring("portrait:".Length).Trim();
                }

                nameText.text = speaker;

                // Set portrait image based on tag
                switch (portraitTag)
                {
                    case "flame_wizard":
                        portraitImage.sprite = flameWizard;
                        break;
                    case "push_wizard":
                        portraitImage.sprite = pushWizard;
                        break;
                    default:
                        portraitImage.sprite = null;
                        break;
                }
            }
            else
            {
                UIManager.Instance.ShowGameUI();
            }
        }

    }
}
