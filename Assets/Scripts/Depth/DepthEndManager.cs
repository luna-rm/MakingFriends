using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class DepthEndManager : MonoBehaviour {

    public static DepthEndManager instance { get; private set; }

    public bool eyeOpen = true;
    public bool startEyeOpen = true;
    public bool inProtection = false;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject Camera;

    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> eyesToSpawn; 
    [SerializeField] private int numberOfObjects = 5;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(60, 5, 60);

    [SerializeField] public float timeAnxiety = 0.2f;

    [SerializeField] private GameObject Pirate;
    [SerializeField] private List<ParticleSystem> EyeParticle;
    [SerializeField] private GameObject Person;


    [SerializeField] private ParticleSystem cry;
    [SerializeField] private ParticleSystem cry1;

    [SerializeField] private CanvasGroup black;

    [SerializeField] private AudioSource CrySounds;

    private bool animStarted = false;
    public Volume postProcessing;
    private UnityEngine.Rendering.Universal.DepthOfField depthOfField;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } 
    }

    private void Start() {
        StartCoroutine(protectionCheck());

        postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.DepthOfField df);
        depthOfField = df;
        depthOfField.active = false;

        var cEmission = cry.emission;
        cEmission.enabled  = false;
        cEmission = cry1.emission;
        cEmission.enabled  = false;
    }

    private IEnumerator protectionCheck() {
        if (eyeOpen) {
            if (!inProtection) {
                if(AnxietyScript.instance.anxiety < 0.99f) {
                    AnxietyScript.instance.addValueToNotKill(true, 0.01f);
                    SpawnEyes();
                }
            }
        }

        yield return new WaitForSeconds(timeAnxiety);
        StartCoroutine(protectionCheck());
    }

    private void Update() {
        if(AnxietyScript.instance.anxiety > 0.99f && !animStarted) {
            Debug.Log("BLEH");
            StopAllCoroutines();
            AnxietyScript.instance.StopAllCoroutines();
            animStarted = true;
            StartCoroutine(anim());
        }
    }

    private IEnumerator anim() {
        GameEventManager.InputContext = InputContextEnum.LOCKED;

        float elapsedTime = 0f;
        Quaternion startRotation = player.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(Pirate.transform.position);

        //while (elapsedTime < 1f) {
        //    float t = elapsedTime / 1f;
        //    Camera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        black.alpha = 1f;
        player.transform.rotation = targetRotation;
        player.transform.position = new Vector3(0, 0, -14f);
        yield return new WaitForSeconds(0.25f);
        black.alpha = 0f;

        depthOfField.active = true;
        
        elapsedTime = 0f;

        startRotation = Camera.transform.rotation;
        targetRotation = startRotation * Quaternion.Euler(30, 0f, 0);

        while (elapsedTime < 1.5f) {
            float t = elapsedTime / 1.5f;
            Camera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            float newY = Mathf.Lerp(0.75f, 0.5f, (elapsedTime / 1.5f));
            Camera.transform.position = new Vector3(Camera.transform.position.x, newY, Camera.transform.position.z);
            float newStart = Mathf.Lerp(100f, 0f, (elapsedTime / 1.5f));
            depthOfField.gaussianStart.value = newStart;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        CrySounds.Play();

        var cEmission = cry.emission;
        cEmission.enabled  = true;
        cEmission = cry1.emission;
        cEmission.enabled  = true;

        elapsedTime = 0f;

        float startZ = Pirate.transform.position.z;

        while (elapsedTime < 3f) {
            float newZ = Mathf.Lerp(startZ, -15.2f, (elapsedTime / 3f));
            Pirate.transform.position = new Vector3(Pirate.transform.position.x, Pirate.transform.position.y, newZ);
            Debug.Log(newZ + " " + Pirate.transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SpawnEyes() {
        for (int i = 0; i < numberOfObjects; i++) {
            Vector3 randomPos = GetRandomPosition();
            
            GameObject newObj = Instantiate(eyesToSpawn[Random.Range(0, eyesToSpawn.Count)], randomPos, Quaternion.identity);
            newObj.transform.localScale = Vector3.one * Random.Range(0.5f, 10f);

            newObj.transform.SetParent(this.transform);
        }
    }

    private Vector3 GetRandomPosition() {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2) + player.transform.position.x;
        float y = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2) + 20f;
        float z = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2) + player.transform.position.z;

        Vector3 offset = new Vector3(x, y, z);

        return transform.position + offset;
    }
}
