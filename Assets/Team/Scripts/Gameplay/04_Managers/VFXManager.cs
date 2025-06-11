using UnityEngine;

public class VFXManager : MonoBehaviour
{

    private GameObject[] _childObjects;
    private ParticleSystem[] _particleSystems;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitialiseChildren();
    }

    [ContextMenu("Initialise Children")]
    public void InitialiseChildren()
    {
        int amountOfChildren = transform.childCount;
        _childObjects = new GameObject[amountOfChildren];
        _particleSystems = new ParticleSystem[amountOfChildren];
        for (int i = 0; i < amountOfChildren; i++)
        {
            _childObjects[i] = transform.GetChild(i).gameObject;
        }
    }

    [ContextMenu("Play Effect")]
    public void EnableParticleEffectChildren()
    {
        for(int i = 0; i < _childObjects.Length; i++)
        {
            _childObjects[i].SetActive(true);
        }
    }

    public void DisableParticleEffectChildren()
    {
        for (int i = 0; i < _childObjects.Length; i++)
        {
            _childObjects[i].SetActive(false);
        }
    }
}
