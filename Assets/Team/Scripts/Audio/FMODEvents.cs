using UnityEngine;
using UnityEngine.Rendering;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience {  get; set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; set; }

    [field: Header("Projectile")]
    [field: SerializeField] public EventReference projectile { get; set; }

    public static FMODEvents instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }


}
