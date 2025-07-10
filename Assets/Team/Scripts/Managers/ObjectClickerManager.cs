using Team.GameConstants;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(20)]
public class ObjectClickerManager : MonoBehaviour
{
    public ObjectClickerManager Instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInputManager.Instance.IsClick)
        {
            LockGhostingVisual();
        }
    }

    public void LockGhostingVisual()
    {
        Ray ray = Camera.main.ScreenPointToRay(GameInputManager.Instance.PointerPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //Add a layer mask to this to stop un needed processing of useless objects.
        {
            if (hit.collider.GetComponent<ObjectClickable>())
            {
                ObjectClickable objClickable = hit.collider.gameObject.GetComponent<ObjectClickable>();
                objClickable.ClickedObject();
            }
        }
    }
}
