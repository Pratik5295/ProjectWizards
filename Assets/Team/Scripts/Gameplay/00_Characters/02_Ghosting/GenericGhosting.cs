using UnityEngine;

public class GenericGhosting : MonoBehaviour
{
    protected Transform ghostingContent;

    protected bool ghostingIsActive;

    [SerializeField] private GameObject _ghostingEffect;
    protected GameObject ghostingEffectRef;


    protected virtual void Awake()
    {
        ghostingContent = transform.GetChild(0);
        InitialiseGhostingContent();
    }

    public virtual void InitialiseGhostingContent()
    {
        ghostingEffectRef = Instantiate(_ghostingEffect, ghostingContent);

        //toggleGhosting();
        disableGhosting();
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

    public void SetGhosting(bool _value)
    {
        ghostingContent?.gameObject.SetActive(_value);
        ghostingIsActive = _value;
    }

}
