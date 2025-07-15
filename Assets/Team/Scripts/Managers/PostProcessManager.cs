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

    public IEnumerator Explosion()
    {
        float elapsedTime = 0.0f;
        float duration = 0.25f;

        float defaultV_Intensity = _vignette.intensity.value;
        float defaultV_Smoothness = _vignette.smoothness.value;

        float targetV_Intensity = 0.4f;
        float targetV_Smoothness = 0.4f;

        float defaultC_Intensity = _chromatic.intensity.value;
        float targetC_Intensity = 0.25f;

        float defaultL_Distortion = _lensDistortion.intensity.value;
        float targetL_Distortion = -0.085f;

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
}
