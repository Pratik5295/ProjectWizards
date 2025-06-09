using UnityEngine;
using UnityEngine.UI;

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


    private void Start()
    {
        numberOfChildren = transform.childCount;

        var horizontalLayout = GetComponent<HorizontalLayoutGroup>();

        spacing = horizontalLayout.spacing;

        SetStartingPos();

        Debug.Log($"Max Position: {CalculateMaxX()}");
    }

    public float CalculateMaxX()
    {
        return startingPosX + numberOfChildren * (cardSize + spacing);
    }

    [ContextMenu("Get Starting Position")]
    public void SetStartingPos()
    {
        startingPosX = transform.GetChild(0).GetComponent<RectTransform>().localPosition.x;

    }
}
