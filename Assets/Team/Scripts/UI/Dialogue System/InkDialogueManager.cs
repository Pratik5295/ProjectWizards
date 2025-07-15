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
        //public TextMeshProUGUI nameText;
        //public Image portraitImage;
        private Story story;

        //TODO: Convert these into a character sprite atlas
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

                // Handle speaker name display
                //if (!string.IsNullOrEmpty(speaker))
                //{
                //    nameText.text = speaker;
                //    nameText.gameObject.SetActive(true);
                //}
                //else
                //{
                //    nameText.gameObject.SetActive(false);
                //}

                // Handle portrait image display
                //if (!string.IsNullOrEmpty(portraitTag))
                //{
                //    portraitImage.sprite = GetPortraitByTag(portraitTag);
                //    portraitImage.gameObject.SetActive(portraitImage.sprite != null);
                //}
                //else
                //{
                //    portraitImage.gameObject.SetActive(false);
                //}
            }
            else
            {
                UIManager.Instance.ShowGameUI();

                //nameText.gameObject.SetActive(false);
               // portraitImage.gameObject.SetActive(false);
            }
        }


        //TODO: Turn this into a separate class or more proper getter system
        private Sprite GetPortraitByTag(string tag)
        {
            switch (tag)
            {
                case "flame_wizard":
                    return flameWizard;
                case "push_wizard":
                    return pushWizard;
                default:
                    return null;
            }
        }

    }
}
