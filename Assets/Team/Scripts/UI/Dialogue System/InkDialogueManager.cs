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
        public const float TypeWriterSpeed = 0.04f;

        public const float MaxScrollHeight = 2000f;
        public const float DialogueScrollSpeed = 25f;
    }
}

namespace Team.UI.DialogueSystem
{
    public class InkDialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI dialogueText;
        public RectTransform dialogueTextRect;

        private float scrollSpeed = MetaConstants.DialogueScrollSpeed;
        private float maxScrollHeight = MetaConstants.MaxScrollHeight;
        private float typewriterSpeed = MetaConstants.TypeWriterSpeed;

        private Story story;
        private bool isScrolling = false;
        private bool isRevealing = false;

        private Coroutine revealCoroutine;

        private string currentLine = "";
        private string fullTextBeforeLine = "";

        public void SetDialogue(TextAsset _asset)
        {
            story = new Story(_asset.text);

            // Reset
            dialogueText.text = "";
            dialogueTextRect.anchoredPosition = new Vector2(0, -Screen.height / 2);
            isScrolling = true;

            ContinueStory();
        }

        public void ContinueStory()
        {
            if (story.canContinue)
            {
                currentLine = story.Continue().Trim();

                if (revealCoroutine != null)
                    StopCoroutine(revealCoroutine);

                revealCoroutine = StartCoroutine(RevealTextOneByOne(currentLine));
            }
            else
            {
                isScrolling = true;
                Invoke(nameof(OnDialogueScrollComplete), MetaConstants.HideDialogueScreenAfter);
            }
        }

        private IEnumerator RevealTextOneByOne(string line)
        {
            isRevealing = true;

            fullTextBeforeLine = dialogueText.text + "\n\n"; // Store text before current line
            dialogueText.text = fullTextBeforeLine;

            for (int i = 0; i <= line.Length; i++)
            {
                dialogueText.text = fullTextBeforeLine + line.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }

            isRevealing = false;

            yield return new WaitForSeconds(MetaConstants.DialoguePauseBetweenLines);
            ContinueStory();
        }


        public void SkipAll()
        {
            if (isRevealing)
            {
                // Stop typewriter coroutine if running
                if (revealCoroutine != null)
                {
                    StopCoroutine(revealCoroutine);
                    revealCoroutine = null;
                }

                isRevealing = false;
                isScrolling = false;

                // Show all remaining text
                while (story.canContinue)
                {
                    string line = story.Continue().Trim();
                    dialogueText.text += "\n\n" + line;
                }

                // Optionally force scroll position to max if you're using a ScrollView
                dialogueTextRect.anchoredPosition = new Vector2(0, 0);

                // Immediately trigger completion logic
                CancelInvoke(nameof(OnDialogueScrollComplete)); // Just in case
                                                                //OnDialogueScrollComplete();
            }

            else
            {
                OnDialogueScrollComplete();
            }
        }


        private void Update()
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
