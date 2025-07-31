using UnityEngine;

[CreateAssetMenu(fileName = "PostEffectsSO", menuName = "Scriptable Objects/PostEffectsSO")]
public class PostEffectsSO : ScriptableObject
{
    [Header("Explosion Post Effect Variables")]

    public float E_ElapsedTime = 0.0f;
    public float E_Duration = 0.25f;

    [Space(8f)]

    public float E_TargetV_Intensity = 0.4f;
    public float E_TargetV_Smoothness = 0.4f;

    [Space(8f)]

    public float E_TargetCA_Intensity = 0.25f;

    [Space(8f)]

    public float E_TargetLENS_Distortion = -0.085f;

    [Space(10f)]


    [Header("Poison Post Effect Variables")]

    public float P_ElapsedTime = 0.0f;
    public float P_Duration = 1f;

    [Space(8f)]

    public float P_TargetV_Intensity = 0.4f;
    public float P_TargetV_Smoothness = 0.4f;
    public Color P_TargetV_Colour = Color.rebeccaPurple;

    [Space(8f)]

    public float P_TargetCA_Intensity = 0.75f;


    [Header("Ice Post Effect Variables")]

    public float I_TargetV_Intensity = 0.4f;
    public Color I_TargetV_Colour = Color.aliceBlue;
}
