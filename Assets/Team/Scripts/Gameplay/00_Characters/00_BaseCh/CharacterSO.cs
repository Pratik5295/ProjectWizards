using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{

    [Header("Tween Values")]
    public float _jumpDuration = .5f;
    public float _jumpHeight = 1f;
    public float _deathShrinkTime = 1f;
    public Vector3 _deathShrinkValue = Vector3.zero;


    [Header("VFX")]
    public GameObject _deathSmokeVFX;
    public GameObject _ignitionVFX;
    public GameObject _jumpVFX;

    [Header("Push VFX Colours")]
    public Color GrassColour;
    public Color IceColour;


    [Header("AnimationCurves")]
    public AnimationCurve _jumpCurve;
    public AnimationCurve _deathCurve;
}
