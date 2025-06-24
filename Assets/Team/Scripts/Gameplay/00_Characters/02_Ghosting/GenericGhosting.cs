using UnityEngine;

public class GenericGhosting : MonoBehaviour
{
    protected Transform ghostingContent;

    protected bool ghostingIsActive;

    [SerializeField] private GameObject _ghostingEffect;
    protected GameObject ghostingEffectRef;


    private void Awake()
    {
        ghostingContent = transform.GetChild(0);
        InitialiseGhostingContent();
    }

    public virtual void InitialiseGhostingContent()
    {
        ghostingEffectRef = Instantiate(_ghostingEffect, ghostingContent);

        toggleGhosting();
    }

    [ContextMenu("Toggle Ghosting Effect")]
    public void toggleGhosting()
    {
        if (!ghostingIsActive) { enableGhosting(); return; }
        disableGhosting();
    }

    [ContextMenu("Enable Ghosting Effect")]
    public void enableGhosting()
    {
        ghostingContent?.gameObject.SetActive(true);
        ghostingIsActive = true;
    }

    [ContextMenu("Disable Ghosting Effect")]
    public void disableGhosting() 
    {
        ghostingContent?.gameObject.SetActive(false);  
        ghostingIsActive = false;
    }

}
