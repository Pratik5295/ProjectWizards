using UnityEngine;
using UnityEngine.Rendering;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience {  get; set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; set; }

    [field: Header("Push")]
    [field: SerializeField] public EventReference s_push { get; set; }

    [field: Header("Fireball")]
    [field: SerializeField] public EventReference s_fireball { get; set; }

    [field: Header("Rotate")]
    [field: SerializeField] public EventReference s_rotate { get; set; }

    [field: Header("Death")]
    [field: SerializeField] public EventReference s_death { get; set; }

    [field: Header("Level Start")]
    [field: SerializeField] public EventReference s_level_select { get; set; }

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
