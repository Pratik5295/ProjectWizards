using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private float cardSize = 200f;

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

            Debug.Log($"Max Position: {endingPosX}");
        }

        public int GetIndex(float _positionX)
        {
            int index = numberOfChildren - 1;   //By default it will go to the last element

            if (_positionX <= startingPosX)
            {
                index = 0;
            }
            else if (_positionX >= endingPosX)
            {
                index = numberOfChildren - 1;
            }
            else
            {

                for (int i = 0; i < cardRanges.Count; i++)
                {
                    //if (_positionX >= cardRanges[i].start && _positionX < cardRanges[i].end)
                    //{
                    //    index =  i;
                    //    return index;
                    //}

                    //Check if the only the start is greater
                    if(_positionX >= cardRanges[i].start)
                    {
                        index = i;

                        //Check if it goes beyond that ranges' end
                        if(_positionX <= cardRanges[i].end)
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

            if(index == -1)
            {
                index = 0;
            }

            return index;
        }

        private void GenerateCardRanges()
        {
            cardRanges.Clear();

            for (int i = 0; i < numberOfChildren; i++)
            {
                float start = startingPosX + i * (cardSize + spacing);
                float end = start + (cardSize/2);

                cardRanges.Add(new CardRange { start = start, end = end });
            }
        }

        public void SetSelected(GameObject _selected)
        {
            selected = _selected;
        }
    }
}
