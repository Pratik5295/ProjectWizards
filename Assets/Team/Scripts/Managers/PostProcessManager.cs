using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager instance;

    private Volume _postProcessVolume;

    [SerializeField]
    private VolumeProfile _currentProfile;

    [Tooltip("Different Post processing Profiles that can be swapped between. Ensure Element 0 is the default profile.")]
    [SerializeField]
    private VolumeProfile[] _Profiles;

    [SerializeField]
    private PostEffectsSO _postEffectsSO;

    private bool isExecutingEffect = false;

    private ChromaticAberration _chromatic;
    private Bloom _bloom;
    private Vignette _vignette;
    private LensDistortion _lensDistortion;


    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else Destroy(gameObject);

        _postProcessVolume = GetComponent<Volume>();
        _currentProfile = _postProcessVolume.profile;
        UpdateEffectsReferences();
    }

    public void SwapProfile(int ProfileInt)
    {
        _currentProfile = _Profiles[ProfileInt];
        UpdateEffectsReferences();
    }

    private void UpdateEffectsReferences()
    {
        foreach (var effect in _currentProfile.components)
        {
            switch (effect)
            {
                case Bloom:
                    _bloom = (Bloom)effect;
                    break;
                case Vignette:
                    _vignette = (Vignette)effect;
                    break;
                case ChromaticAberration:
                    _chromatic = (ChromaticAberration)effect;
                    break;
                case LensDistortion:
                    _lensDistortion = (LensDistortion)effect;
                    break;
            }
        }

    }

    #region Public Adjustment Functions

    public void ChromaticAberationAdjustment(float Intensity)
    {
        if (_chromatic)
        {
            _chromatic.intensity.value = Intensity;
        }
    }

    public void BloomAdjustment(float Threshold, float Intensity, Vector4 Tint)
    {
        if (_bloom)
        {
            _bloom.threshold.value = Threshold;
            _bloom.intensity.value = Intensity;

            Color colour = (Color)_bloom.tint;
            if (Tint != null)
            {
                colour = new Color(Tint.x, Tint.y, Tint.z);
                colour.a = Tint.w;
            }
            _bloom.tint = new ColorParameter(colour);
        }
    }

    public void VignetteAdjustment(Vector4 Colour, float Intensity, float Smoothness)
    {
        if (_vignette)
        {
            Color colour = (Color)_vignette.color;
            if (Colour != null)
            {
                colour = new Color(Colour.x, Colour.y, Colour.z);
                colour.a = Colour.w;
            }

            _vignette.color = new ColorParameter(colour);

            _vignette.intensity.value = Intensity;
            _vignette.smoothness.value = Smoothness; 
        }
    }
    #endregion

    public void Explode()
    {
        StartCoroutine(Explosion());
    }
    public void Poisoned()
    {
        if (isExecutingEffect) return;
        StartCoroutine(PoisonScreen());
    }


    #region PostProcessingEffects
    public IEnumerator Explosion()
    {
        float elapsedTime = _postEffectsSO.E_ElapsedTime;
        float duration = _postEffectsSO.E_Duration;

        float defaultV_Intensity = _vignette.intensity.value;
        float defaultV_Smoothness = _vignette.smoothness.value;

        float targetV_Intensity = _postEffectsSO.E_TargetV_Intensity;
        float targetV_Smoothness = _postEffectsSO.E_TargetV_Smoothness;

        float defaultC_Intensity = _chromatic.intensity.value;
        float targetC_Intensity = _postEffectsSO.E_TargetCA_Intensity;

        float defaultL_Distortion = _lensDistortion.intensity.value;
        float targetL_Distortion = _postEffectsSO.E_TargetLENS_Distortion;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _vignette.intensity.value = Mathf.Lerp(defaultV_Intensity, targetV_Intensity, t);
            _vignette.smoothness.value = Mathf.Lerp(defaultV_Smoothness, targetV_Smoothness, t);
            _chromatic.intensity.value = Mathf.Lerp(defaultC_Intensity, targetC_Intensity, t);
            _lensDistortion.intensity.value = Mathf.Lerp(defaultL_Distortion, targetL_Distortion, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        
        elapsedTime = 0.0f;
        while (elapsedTime < duration)
        { 
            float t = elapsedTime / duration;
            _vignette.intensity.value = Mathf.Lerp(targetV_Intensity, defaultV_Intensity, t);
            _vignette.smoothness.value = Mathf.Lerp(targetV_Smoothness, defaultV_Smoothness, t);
            _chromatic.intensity.value = Mathf.Lerp(targetC_Intensity, defaultC_Intensity, t);
            _lensDistortion.intensity.value = Mathf.Lerp(targetL_Distortion, defaultL_Distortion, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _vignette.intensity.value = defaultV_Intensity;
        _vignette.smoothness.value = defaultV_Smoothness;
        _chromatic.intensity.value = defaultC_Intensity;
    }

    public IEnumerator PoisonScreen()
    {
        isExecutingEffect = true;

        float elapsedTime = _postEffectsSO.P_ElapsedTime;
        float duration = _postEffectsSO.P_Duration;

        float defaultV_Intensity = _vignette.intensity.value;
        float defaultV_Smoothness = _vignette.smoothness.value;
        Color defaultV_Colour = _vignette.color.value;

        float targetV_Intensity = _postEffectsSO.P_TargetV_Intensity;
        float targetV_Smoothness = _postEffectsSO.P_TargetV_Smoothness;
        Color targetV_Colour = _postEffectsSO.P_TargetV_Colour;

        float defaultC_Intensity = _chromatic.intensity.value;
        float targetC_Intensity = _postEffectsSO.P_TargetCA_Intensity;

        _vignette.color.value = targetV_Colour;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _vignette.intensity.value = Mathf.Lerp(defaultV_Intensity, targetV_Intensity, t);
            _vignette.smoothness.value = Mathf.Lerp(defaultV_Smoothness, targetV_Smoothness, t);
            _chromatic.intensity.value = Mathf.Lerp(defaultC_Intensity, targetC_Intensity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.35f);

        elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _vignette.intensity.value = Mathf.Lerp(targetV_Intensity, defaultV_Intensity, t);
            _vignette.smoothness.value = Mathf.Lerp(targetV_Smoothness, defaultV_Smoothness, t);
            _chromatic.intensity.value = Mathf.Lerp(targetC_Intensity, defaultC_Intensity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _vignette.color.value = defaultV_Colour;
        _vignette.intensity.value = defaultV_Intensity;
        _vignette.smoothness.value = defaultV_Smoothness;
        _chromatic.intensity.value = defaultC_Intensity;

        isExecutingEffect = false;
    }
    #endregion
}
