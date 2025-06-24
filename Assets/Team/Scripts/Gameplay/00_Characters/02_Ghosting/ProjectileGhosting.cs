using UnityEngine;
using UnityEngine.Rendering;

public class ProjectileGhosting : GenericGhosting
{
    public float maxProjectDist = 66f;

    private Line_Length projectilePath;

    public override void InitialiseGhostingContent()
    {
        base.InitialiseGhostingContent();

        projectilePath = ghostingEffectRef.GetComponent<Line_Length>();
    }

    public void SetProjectionValue(float rangeToFlip)
    {
        maxProjectDist = rangeToFlip * -1;
    }

    private void Update()
    {
        if (ghostingIsActive)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxProjectDist))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blueViolet);
                float NewLineLength = Vector3.Distance(transform.position, hit.transform.position);
                projectilePath.lineLength = NewLineLength;
                projectilePath.ValidateLength();
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxProjectDist, Color.red);
                float NewLineLength = maxProjectDist / 10;
                projectilePath.lineLength = NewLineLength;
                projectilePath.ValidateLength();
            }
        }
    }
}
