using System.Collections.Generic;
using UnityEngine;

namespace Team.UI
{
    public class CharacterBark : MonoBehaviour
    {
        [SerializeField] private TextAsset inkFile;

        private Dictionary<string, List<string>> tagToLines;

        void Awake()
        {
            tagToLines = new Dictionary<string, List<string>>();
            ParseInkText(inkFile.text);
        }

        private void ParseInkText(string text)
        {
            string[] lines = text.Split('\n');
            string currentTag = null;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                // Tag line
                if (line.StartsWith("#"))
                {
                    currentTag = line.Substring(1).Trim();
                    if (!tagToLines.ContainsKey(currentTag))
                        tagToLines[currentTag] = new List<string>();
                }
                // Choice line (e.g., {~option1|option2|...})
                else if (line.StartsWith("{~") && currentTag != null)
                {
                    string optionsBlock = line.Trim('{', '}', '~');
                    string[] options = optionsBlock.Split('|');
                    foreach (string option in options)
                    {
                        tagToLines[currentTag].Add(option.Trim());
                    }
                }
            }
        }

        public string GetRandomLine(string tag)
        {
            if (tagToLines.ContainsKey(tag))
            {
                List<string> lines = tagToLines[tag];
                if (lines.Count > 0)
                {
                    return lines[Random.Range(0, lines.Count)];
                }
            }
            return $"[No lines found for tag '{tag}']";
        }
    }
}
