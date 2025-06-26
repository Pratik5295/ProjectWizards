
using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using static Team.GameConstants.MetaConstants;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public enum BarkTag
        {
            OnClick,
            OnCast,
            OnAbsorb,
            OnFirecast,
            OnWindcast,
            OnFailcast
        }
    }
}


namespace Team.UI
{
    public class CharacterBark : MonoBehaviour
    {
        [SerializeField] private TextAsset inkFile;

        private Story story;
        public Dictionary<BarkTag, List<string>> barkMap = new();

        private List<string> barkKnots = new List<string> { "OnClick", "OnCast", "OnAbsorb", "OnFirecast" , "OnWindcast" , "OnFailcast" };

        private void Start()
        {
            story = new Story(inkFile.text);
            ExtractLists(barkKnots);
        }

        private void ExtractLists(List<string> keys)
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

                BarkTag tag = GetTag(key);
                barkMap[tag] = values;
            }
        }

        private void PrintList(BarkTag key)
        {
            if (barkMap.TryGetValue(key, out var list))
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
            if (barkMap.ContainsKey(tag))
            {
                List<string> lines = barkMap[tag];
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
                case "OnClick":
                    return BarkTag.OnClick;
                case "OnCast":
                    return BarkTag.OnCast;
                case "OnAbsorb":
                    return BarkTag.OnAbsorb;
                case "OnFirecast":
                    return BarkTag.OnFirecast;
                case "OnWindcast":
                    return BarkTag.OnWindcast;
                case "OnFailcast":
                    return BarkTag.OnFailcast;

                default:
                    return BarkTag.OnClick;
            }
        }
    }
}
