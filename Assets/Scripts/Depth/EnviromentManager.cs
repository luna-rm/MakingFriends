using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Enviroment : MonoBehaviour
{
    [Header("Fog Settings")]
    [SerializeField] private bool enableFog = true;
    [SerializeField] private Color fogColor = Color.black;
    [SerializeField] private float fogDensityLight = 0.06f; // For Exponential modes

    [Header("Fog Mode")]
    [SerializeField] private FogMode fogMode = FogMode.ExponentialSquared;

    [Header("things to change")]
    [SerializeField] private Light RedLight;
    public Volume postProcessing;
    private ColorAdjustments colorAdjustments;
    private SplitToning splitToning;

    [Header("Light Mode")]
    [SerializeField] private float transitionTimeToL = 2.0f;
    [SerializeField] private float lightIntensityL = 100f;
    [SerializeField] private float postExposureL = 0f;
    [SerializeField] private float contrastL = 0f;

    [Header("Dark Mode")]
    [SerializeField] private float transitionTimeToD = 2.0f;
    [SerializeField] private float lightIntensityD = 0f;
    [SerializeField] private float postExposureD = -1f;
    [SerializeField] private float contrastD = 100f;

    [Header("Wait")]
    [SerializeField] private float transitionTimeWaitToDark = 4f;
    [SerializeField] private float transitionTimeWaitToLigh = 12f;

    private Coroutine atCoroutine = null;

    private bool toDark = true;

    private void Start() {
        ApplyFogSettings();

        postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.ColorAdjustments ca);
        colorAdjustments = ca;

        postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.SplitToning st);
        splitToning = st;

        StartCoroutine(init());
    }

    private IEnumerator init() {
        yield return new WaitForSeconds(1f);
        atCoroutine = StartCoroutine(transitionWait());
    }

    private IEnumerator transitionWait() {
        Debug.Log("W");
        if (toDark) {
            yield return new WaitForSeconds(transitionTimeWaitToDark);
            atCoroutine = StartCoroutine(transitionToDark());
        } else {
            yield return new WaitForSeconds(transitionTimeWaitToLigh);
            atCoroutine = StartCoroutine(transitionToLight());
        }
    }


    private IEnumerator transitionToLight() {
        Debug.Log("L");
        DepthManager.instance.startParticles = false;
        DepthManager.instance.SpawnEyes();
        DepthManager.instance.startEyeOpen = true;

        float elapsedTime = 0f;

        while (elapsedTime < transitionTimeToL) {
            float newLightIntensity = Mathf.Lerp(lightIntensityD, lightIntensityL, (elapsedTime / transitionTimeToL));
            float newPostExposure = Mathf.Lerp(postExposureD, postExposureL, (elapsedTime / transitionTimeToL));
            float newContrast = Mathf.Lerp(contrastD, contrastL, (elapsedTime / transitionTimeToL));
            RedLight.intensity = newLightIntensity;
            colorAdjustments.postExposure.value = newPostExposure;
            colorAdjustments.contrast.value = newContrast;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        DepthManager.instance.eyeOpen = true;
        toDark = true;
        atCoroutine = StartCoroutine(transitionWait());
    }

    private IEnumerator transitionToDark() {
        Debug.Log("D");
        DepthManager.instance.eyeOpen = false;
        DepthManager.instance.startEyeOpen = false;

        float elapsedTime = 0f;

        while (elapsedTime < transitionTimeToD) {
            float newLightIntensity = Mathf.Lerp(lightIntensityL, lightIntensityD, (elapsedTime / transitionTimeToD));
            float newPostExposure = Mathf.Lerp(postExposureL, postExposureD, (elapsedTime / transitionTimeToD));
            float newContrast = Mathf.Lerp(contrastL, contrastD, (elapsedTime / transitionTimeToD));
            RedLight.intensity = newLightIntensity;
            colorAdjustments.postExposure.value = newPostExposure;
            colorAdjustments.contrast.value = newContrast;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        toDark = false;
        atCoroutine = StartCoroutine(transitionWait());
        DepthManager.instance.startParticles = true;
    }

    // Call this to update fog at runtime
    public void ApplyFogSettings() {
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogDensity = fogDensityLight;
        
    }

    // Helper method to change color smoothly via script
    public void SetFogColor(Color newColor)
    {
        RenderSettings.fogColor = newColor;
    }

    // Helper to change density
    public void SetFogDensity(float density)
    {
        RenderSettings.fogDensity = density;
    }
}
