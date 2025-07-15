using System.Collections;
using Ink.Runtime;
using Team.GameConstants;
using Team.Managers;
using TMPro;
using UnityEngine;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float HideDialogueScreenAfter = 2f;
        public const float DialoguePauseBetweenLines = 1f;
        public const float TypeWriterSpeed = 0.35f;

        public const float MaxScrollHeight = 2000f;
        public const float DialogueScrollSpeed = 20f;
    }
}

namespace Team.UI.DialogueSystem
{
    public class InkDialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI dialogueText;
        public RectTransform dialogueTextRect; // RectTransform of the Text object
        private float scrollSpeed = MetaConstants.DialogueScrollSpeed;
        private float maxScrollHeight = MetaConstants.MaxScrollHeight;
        private float typewriterSpeed = MetaConstants.TypeWriterSpeed; // seconds between characters

        private Story story;
        private bool isScrolling = false;
        private bool isRevealing = false;

        private Coroutine revealCoroutine;

        public void SetDialogue(TextAsset _asset)
        {
            story = new Story(_asset.text);

            // Reset everything
            dialogueText.text = "";
            dialogueTextRect.anchoredPosition = new Vector2(0, -Screen.height/2); // start below
            isScrolling = true;

            ContinueStory();
        }

        public void ContinueStory()
        {
            if (story.canContinue)
            {
                string nextLine = story.Continue().Trim();

                // Start the typewriter effect for the next line
                if (revealCoroutine != null)
                    StopCoroutine(revealCoroutine);

                revealCoroutine = StartCoroutine(RevealTextOneByOne(nextLine));
            }
            else
            {
                isScrolling = true; // Let final scroll finish

                Invoke(nameof(OnDialogueScrollComplete), MetaConstants.HideDialogueScreenAfter);
            }
        }

        private IEnumerator RevealTextOneByOne(string line)
        {
            isRevealing = true;

            string existingText = dialogueText.text;
            string newLine = "\n"; // Formatting between paragraphs
            string fullText = existingText + newLine;
            dialogueText.text = fullText;

            for (int i = 0; i <= line.Length; i++)
            {
                dialogueText.text = fullText + line.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }

            isRevealing = false;

            // Auto-continue after typing is done
            yield return new WaitForSeconds(MetaConstants.DialoguePauseBetweenLines); // Optional pause after line
            ContinueStory();
        }

        void Update()
        {
            if (isScrolling)
            {
                dialogueTextRect.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

                if (dialogueTextRect.anchoredPosition.y > maxScrollHeight && !story.canContinue && !isRevealing)
                {
                    isScrolling = false;
                   
                }
            }
        }

        private void OnDialogueScrollComplete()
        {
            Debug.Log("Story end point reached");
            UIManager.Instance.ShowGameUI();
        }
    }
}
