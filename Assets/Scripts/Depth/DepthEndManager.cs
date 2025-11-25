using System.Collections;
using System.Collections.Generic;
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

    private bool animStarted = false;
    public Volume postProcessing;
    private UnityEngine.Rendering.Universal.DepthOfField depthOfField;


    private void Awake() {
        if(instance == null) {
            instance = this;
        } 
    }

    private void Start() {
        if(GameEventManager.InputContext == InputContextEnum.DEFAULT) {
            StartCoroutine(protectionCheck());
        }

        postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.DepthOfField df);
        depthOfField = df;
        depthOfField.active = false;
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
        Quaternion startRotation = Camera.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(Pirate.transform.position);

        AnxietyScript.instance.anxiety = 0.1f;
        StartCoroutine(AnxietyScript.instance.textAppear(3));

        while (elapsedTime < 0.5f) {
            float t = elapsedTime / 0.5f;
            Camera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        depthOfField.active = true;


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
