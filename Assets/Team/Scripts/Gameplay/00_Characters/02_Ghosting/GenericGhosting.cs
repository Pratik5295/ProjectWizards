using UnityEngine;

public class GenericGhosting : MonoBehaviour
{
    protected Transform ghostingContent;

    public bool ghostingIsActive;

    public bool isHovered = false;

    [SerializeField]
    private bool _lockGhosting = false;   //Sets lock on the ghosting effect

    public bool IsLocked => !_lockGhosting;

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
        
        if (!ghostingIsActive) { enableGhosting();   return; }
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
        if (_lockGhosting) return;

        if (_value)
        {
            enableGhosting();
        }
        else
        {
            disableGhosting();
        }
    }

    public void RefreshGhosting()
    {
        if (_lockGhosting)
            enableGhosting();
        else
            disableGhosting();
    }

    public void ToggleGhostingLock()
    {
        //Toggle the lock
        _lockGhosting = !_lockGhosting;
    }

}
