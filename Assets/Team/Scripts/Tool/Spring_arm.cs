using Team.Gameplay.GridSystem;
using UnityEngine;


public class Spring_arm : MonoBehaviour
{





    [SerializeField]
    private float spring_Arm_Length;

    [SerializeField]
    private GameObject camera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Sets camera's position to match spring arm length.
    //Sets camera's rotation to look to spring arm origin.
    private void OnValidate()
    {
        Vector3 target_Pos = transform.position + (transform.forward * spring_Arm_Length);

        camera.transform.position = target_Pos;
        camera.transform.LookAt(transform.position);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, camera.transform.position);
    }


    //Sets spring arm camera to set angle that works with orthogonal camera for our levels.
    [ContextMenu("Snap to Orthogonal Angle")]
    private void SnapToOrthogonalView()
    {

        transform.rotation = Quaternion.Euler(-45, -135, 0);


    }

    [ContextMenu("Snap Camera to Grid Centre")]
    private void SnapToGridCentre()
    {

        GridManager gm = GameObject.FindAnyObjectByType<GridManager>();

        if (gm != null)
        {

            Debug.Log("Found!");

        }

    }


}
