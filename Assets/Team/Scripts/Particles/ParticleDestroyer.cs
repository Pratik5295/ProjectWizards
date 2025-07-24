using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    private VFXManager _myManager;

    public void Initialise(VFXManager VFXmanager)
    {
        _myManager = VFXmanager;
    }

    public void OnParticleSystemStopped()
    {
        _myManager.CleanUpEffect();
    }
}
