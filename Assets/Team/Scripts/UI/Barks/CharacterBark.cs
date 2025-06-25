using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace Team.UI
{
    public enum BarkTag
    {
        OnClick,
        OnAbsorb,
        OnFirecast,
        OnWindcast,
        OnFailcast
    }

    public class CharacterBark : MonoBehaviour
    {
        [SerializeField] private TextAsset inkFile;

        private Dictionary<BarkTag, List<string>> tagToLines;

        void Awake()
        {
            tagToLines = new Dictionary<BarkTag, List<string>>();
            //ParseInkText(inkFile.text);

            ParseInkFile(inkFile.text);
        }

        private void ParseInkText(string text)
        {
            Story story = new Story(inkFile.text);

            story.onError += (msg, type) =>
            {
                if (type == Ink.ErrorType.Warning)
                    Debug.LogWarning(msg);
                else
                    Debug.LogError(msg);
            };

            
            var barkLists = story.listDefinitions.lists;
            Debug.Log($"Pratik {barkLists.Count} ");

            //Tag: On Click , On Fail, On Fire etc
            foreach(var bark in barkLists)
            {
                BarkTag tag = GetTag(bark.name);
                List<string> barkLines = new List<string>();
                //The contents of each list
                foreach (var item in bark.items) 
                {
                    Debug.Log($"Pratik Value: {item.Key.itemName}");
                    barkLines.Add(item.Key.itemName);
                }

                tagToLines.Add(tag, barkLines); 
            }

            foreach(var tag in tagToLines)
            {
                Debug.Log($"Pratik Tag:{tag.Key} and Values: {tag.Value.Count}");
            }

            

            Debug.Log($"Bark loaded successfully. Count: {tagToLines.Count}");
        }


        public Dictionary<string, List<string>> barkTable = new();
        private void ParseInkFile(string inkText)
        {
            string[] lines = inkText.Split('\n');
            string currentKnot = null;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                // Start of a knot, like === onClick ===
                if (line.StartsWith("===") && line.EndsWith("==="))
                {
                    currentKnot = line.Trim('=').Trim();
                    barkTable[currentKnot] = new List<string>();
                }
                // If a line starts with {~ and we're in a knot, extract the choices
                else if (line.StartsWith("{~") && currentKnot != null)
                {
                    string content = line.Trim('{', '}', '~');
                    string[] options = content.Split('|');

                    foreach (var option in options)
                    {
                        string trimmed = option.Trim();
                        if (!string.IsNullOrEmpty(trimmed))
                        {
                            barkTable[currentKnot].Add(trimmed);
                        }
                    }
                }
            }

                foreach (var kvp in barkTable)
            {
                Debug.Log($"Knot: {kvp.Key}");
                foreach (var line in kvp.Value)
                {
                    Debug.Log($"  - {line}");
                }
            }
        }

        public string GetRandomBark(BarkTag tag)
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

        private BarkTag GetTag(string _value)
        {
            switch (_value)
            {
                case "on_click":
                    return BarkTag.OnClick;
                case "on_absorb":
                    return BarkTag.OnAbsorb;
                case "on_firecast":
                    return BarkTag.OnFirecast;
                case "on_windcast":
                    return BarkTag.OnWindcast;
                case "on_failcast":
                    return BarkTag.OnFailcast;

                default:
                    return BarkTag.OnClick;
            }
        }
    }
}
