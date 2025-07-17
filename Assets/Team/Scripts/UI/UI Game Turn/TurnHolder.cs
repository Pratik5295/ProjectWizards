using System;
using System.Collections.Generic;
using Team.GameConstants;
using Team.UI;
using Team.UI.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Team.GameConstants
{
    public static partial class MetaConstants
    {
        public const float CardDiviFactor = 2.86f;
    }
}

namespace Team.Gameplay.TurnSystem
{
    [System.Serializable]
    public struct CardRange
    {
        public float start;
        public float end;
    }

    [DefaultExecutionOrder(5)]
    public class TurnHolder : MonoBehaviour
    {
        private int numberOfChildren;

        [SerializeField]
        private float cardSize;

        [SerializeField]
        private float spacing;

        [SerializeField]
        private float startingPosX;

        [SerializeField] private float endingPosX;

        [SerializeField]
        private List<CardRange> cardRanges = new List<CardRange>();

        [SerializeField]
        private GameObject selected = null;

        public bool HasSelected => selected != null;

        public Action OnTurnOrderUpdatedEvent;

        [Header("Breakpoint Variable System")]

        [SerializeField]
        private int minIndex = 0;

        [Space(5)]
        [Header("Ghost Card Reference")]

        [SerializeField]
        private GhostCard ghostCard;

        [ContextMenu("Get Starting Position")]
        public void CalculateExtremePositions()
        {
            startingPosX = transform.GetChild(0).GetComponent<RectTransform>().localPosition.x;
            endingPosX = startingPosX + numberOfChildren * (cardSize + spacing);

        }

        public void InitializeComplete()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

            numberOfChildren = transform.childCount;

            var horizontalLayout = GetComponent<HorizontalLayoutGroup>();

            spacing = horizontalLayout.spacing;

            CalculateExtremePositions();

            GenerateCardRanges();

            DeactivateGhostCard();

            Debug.Log($"Max Position: {endingPosX}");
        }

        public int GetIndex(float _positionX)
        {
            int index = numberOfChildren - 1;   //By default it will go to the last element

            if (_positionX <= startingPosX)
            {
                index = minIndex;
            }
            else if (_positionX >= endingPosX)
            {
                index = numberOfChildren - 1;
            }
            else
            {

                for (int i = 0; i < cardRanges.Count; i++)
                {
                    //Check if the only the start is greater
                    if (_positionX >= cardRanges[i].start)
                    {
                        index = i;

                        //Check if it goes beyond that ranges' end
                        if (_positionX <= cardRanges[i].end)
                        {
                            return index;
                        }
                        else
                        {
                            index++;
                            continue;
                        }
                    }
                }
            }

            if (index == -1)
            {
                index = minIndex;
            }

            return index;
        }

        private void GenerateCardRanges()
        {
            cardRanges.Clear();

            for (int i = 0; i < numberOfChildren; i++)
            {
                float start = startingPosX + i * (cardSize + spacing);
                float end = start + (cardSize / MetaConstants.CardDiviFactor);

                cardRanges.Add(new CardRange { start = start, end = end });
            }
        }

        public void RemoveCardRanges(int _count)
        {
            cardRanges.RemoveRange(0, _count);
        }

        public void SetSelected(GameObject _selected)
        {
            selected = _selected;
        }

        public void Reset()
        {
            minIndex = 0;

            CalculateExtremePositions();
            GenerateCardRanges();

        }

        public void BreakpointInitiate(int _breakpointIndex)
        {
            minIndex = _breakpointIndex + 1;

            startingPosX = transform.GetChild(minIndex).GetComponent<RectTransform>().localPosition.x + (spacing * 2);
        }


        #region Ghost Card Section

        public void ActivateGhostCard(UIGameCard _card)
        {
            int index = _card.transform.GetSiblingIndex();

            ghostCard.transform.SetSiblingIndex(index);
            ghostCard.gameObject.SetActive(true);

            ghostCard.PairCardWith(_card);
        }

        public void DeactivateGhostCard()
        {
            ghostCard.gameObject.SetActive(false);

            ghostCard.ResetGhostCard();
        }

        public void SetGhostIndex(int _index)
        {
            ghostCard.transform.SetSiblingIndex(_index);
        }


        #endregion
    }
}
