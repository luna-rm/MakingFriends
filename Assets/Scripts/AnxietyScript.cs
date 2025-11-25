using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class AnxietyScript : MonoBehaviour {

    public static AnxietyScript instance { get; private set; }

    [Range(0f, 1f)]
    public float anxiety = 0f;

    public Volume postProcessing;

    private UnityEngine.Rendering.Universal.Vignette vignette;
    private UnityEngine.Rendering.Universal.ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    public float valueTo = 0f;

    public Coroutine currentAnimCoroutine;

    [SerializeField] private List<string> msg = new List<string>();

    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private float maxDistance = 7.5f;

    [SerializeField] private GameObject textField;

    public List<GameObject> textLines = new List<GameObject>();

    [SerializeField] bool isDepth = false;
    [SerializeField] public float timeAnxiety = 0.2f;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } 
    }

    void Start() {
        if (postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.Vignette vg)) {
            vignette = vg;
        }
        else {
            Debug.LogError("Vignette component not found on the Volume profile.");
        }

        if (postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.FilmGrain fg)) {
            filmGrain = fg;
        }
        else {
            Debug.LogError("FilmGrain component not found on the Volume profile.");
        }

        if (postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.ChromaticAberration ca)) {
            chromaticAberration = ca;
        }
        else {
            Debug.LogError("Vignette component not found on the Volume profile.");
        }

        if(DepthManager.instance != null) {
            timeAnxiety = DepthManager.instance.timeAnxiety;
        }
    }

    void Update() {

        if (filmGrain != null) {
            filmGrain.intensity.value = anxiety;
        }

        if (chromaticAberration != null) {
            chromaticAberration.intensity.value = anxiety;
        }

        float targetIntensity = Mathf.Lerp(anxiety/8+0.1f + anxiety/10, anxiety/8+0.15f + anxiety/10, Mathf.PingPong(Time.time * 0.33f, 1));
        vignette.intensity.value = targetIntensity;

        if(anxiety >= 1) {
            GameEventManager.instance.die();
        }

        if(anxiety == 0f){
            foreach (GameObject obj in textLines) {
                if (obj != null) {
                    Destroy(obj);
                }
            }
            StopAllCoroutines();
        }
    }

    public void addValue(bool anim, float add) {
        if (currentAnimCoroutine != null) {
            StopCoroutine(currentAnimCoroutine);
            anxiety = valueTo;
        }
        
        if (anim) {
            if(add < 0.1f) {
                currentAnimCoroutine = StartCoroutine(addLowValueAnim(add));
            } else {
                currentAnimCoroutine = StartCoroutine(addValueAnim(add));
            }
        } else {
            anxiety += add;
        }
    }

    public void addValueToNotKill(bool anim, float add) {
        if (currentAnimCoroutine != null) {
            StopCoroutine(currentAnimCoroutine);

            if(valueTo < 1f) {
                anxiety = valueTo;
            } else {
                anxiety = 0.99f;
            }
        }

        if (anxiety + add >= 1f) {
            add = 0.99f - anxiety;
        }
        
        if (anim) {
            if(add < 0.1f) {
                currentAnimCoroutine = StartCoroutine(addLowValueAnim(add));
            } else {
                currentAnimCoroutine = StartCoroutine(addValueAnim(add));
            }
        } else {
            anxiety += add;
        }
    }
    
    public IEnumerator addLowValueAnim(float add) {
        valueTo = anxiety + add;
        Debug.Log(valueTo);
        anxiety += 0.25f;
        if(anxiety >= 1f) {
            anxiety = 0.99f;
        }
        if(Mathf.RoundToInt(valueTo * 1000) % (10 - Mathf.RoundToInt(valueTo * 9)) == 0){
            StartCoroutine(textAppear(1));
        }

        yield return new WaitForSeconds(timeAnxiety);
        anxiety = valueTo;
    }

    public IEnumerator addValueAnim(float add) {
        valueTo = anxiety + add;
        Debug.Log(valueTo);
        anxiety = 0.99f;

        float elapsedTime = 0f;

        StartCoroutine(textAppear((int) (valueTo * 7)));

        while (elapsedTime < 2f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < 2f) {
            float newValue = Mathf.Lerp(0.99f, valueTo, (elapsedTime / 2f));
            anxiety = newValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        anxiety = valueTo;
        currentAnimCoroutine = null;        
    }

    public IEnumerator textAppear(int howMany) {
        Debug.Log(howMany);
        if(howMany < 1){
            howMany = 1;
        }
        for(int i = 0; i < howMany; i++) {
            if(anxiety > 0) {
                awakeTextAppear();
                yield return new WaitForSeconds(2f/howMany + 0.25f);
            }
        }
    }

    private void awakeTextAppear() {
        int text = UnityEngine.Random.Range(0, msg.Count);

        RaycastHit hitInfo;
        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        bool didHit = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hitInfo, maxDistance);

        if (didHit) {
            spawnPosition = hitInfo.point;
            spawnRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal); 
        }
        else {
            spawnPosition = cameraTransform.position + cameraTransform.forward * maxDistance; 
        }

        spawnPosition.y += (UnityEngine.Random.value) * 3f + 1f;

        if (isDepth) {
            spawnPosition.y += (UnityEngine.Random.value - 0.5f) * 4f;
        }

        spawnPosition.x += (UnityEngine.Random.value - 0.5f) * 7f;
        spawnPosition.z += (UnityEngine.Random.value - 0.5f) * 7f;       

        GameObject textObj = Instantiate(textField, spawnPosition, spawnRotation);
        textObj.GetComponent<TextMeshPro>().enabled = true;
        textObj.GetComponent<TextMeshPro>().fontSize = 3;
        textObj.GetComponent<TextScript>().isDialogue = false;
        textObj.GetComponent<TextScript>().DisplayText(msg[text]);

        textLines.Add(textObj);

    }
}
