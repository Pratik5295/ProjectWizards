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
            ParseInkText(inkFile.text);
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

            //InkList newValue = new InkList("colors.red", story.listDefinitions);

            //string[] lines = text.Split('\n');
            //string tagString = null;

            //foreach (string rawLine in lines)
            //{
            //    string line = rawLine.Trim();

            //    // Tag line
            //    if (line.StartsWith("#"))
            //    {
            //        tagString = line.Substring(1).Trim();

            //        var tag = GetTag(tagString);

            //        if (!tagToLines.ContainsKey(tag))
            //            tagToLines[tag] = new List<string>();
            //    }
            //    // Choice line (e.g., {~option1|option2|...})
            //    else if (line.StartsWith("{~") && tagString != null)
            //    {
            //        string optionsBlock = line.Trim('{', '}', '~');
            //        string[] options = optionsBlock.Split('|');

            //        var tag = GetTag(tagString);
            //        foreach (string option in options)
            //        {
            //            tagToLines[tag].Add(option.Trim());
            //        }
            //    }
            //}

            //foreach(var tag in story.globalTags)
            //{
            //    Debug.Log($"{tag} : {tag.Substring("on_click:".Length).Trim()}");
            //}
            var barkLists = story.listDefinitions.lists;
            Debug.Log($"Pratik {barkLists.Count} and ");

            foreach(var bark in barkLists)
            {
                foreach (var item in bark.items) 
                {
                    Debug.Log($"Value: {item.Key.fullName}");
                }
            }

            List<string> barkLines = new List<string>();

            

            Debug.Log($"Bark loaded successfully. Count: {tagToLines.Count}");
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
