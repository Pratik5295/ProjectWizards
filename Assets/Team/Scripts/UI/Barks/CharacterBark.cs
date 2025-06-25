
using Ink.Runtime;
using System.Collections.Generic;
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

        private Story story;
        public Dictionary<string, List<string>> inkStringLists = new();

        void Start()
        {
            story = new Story(inkFile.text);
            ExtractLists(new List<string> { "OnClick", "OnHover" });

            Debug.Log("Type: On Click");
            PrintList("OnClick");

            Debug.Log("Type: On Hover");
            PrintList("OnHover");
        }

        void ExtractLists(List<string> keys)
        {
            foreach (var key in keys)
            {
                // Try to jump to a knot named "key"
                bool pathExists = story.mainContentContainer.namedContent.ContainsKey(key);
                if (!pathExists)
                {
                    Debug.LogWarning($"Knot '{key}' not found in Ink.");
                    continue;
                }

                story.ChoosePathString(key);
                List<string> values = new();

                while (story.canContinue)
                {
                    string line = story.Continue().Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith("-"))
                            line = line.Substring(1).Trim();
                        values.Add(line);
                    }
                }

                inkStringLists[key] = values;
            }
        }

        void PrintList(string key)
        {
            if (inkStringLists.TryGetValue(key, out var list))
            {
                foreach (var line in list)
                {
                    Debug.Log(line);
                }
            }
            else
            {
                Debug.LogWarning($"No list found for key: {key}");
            }
        }

        public string GetRandomBark(BarkTag tag)
        {
            //if (tagToLines.ContainsKey(tag))
            //{
            //    List<string> lines = tagToLines[tag];
            //    if (lines.Count > 0)
            //    {
            //        return lines[Random.Range(0, lines.Count)];
            //    }
            //}
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
