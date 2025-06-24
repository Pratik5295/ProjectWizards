using UnityEngine;

public class Line_Length : MonoBehaviour
{

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject outline;

    [SerializeField]
    public float lineLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ValidateLength()
    {
        Vector3 current_Loc = lineRenderer.GetPosition(1);
        Vector3 target_Loc = new Vector3(current_Loc.x, current_Loc.y, lineLength);
        

        lineRenderer.SetPosition(1, target_Loc);
        outline.transform.localPosition = new Vector3(target_Loc.x, outline.transform.localPosition.y, target_Loc.z);

    }

}
