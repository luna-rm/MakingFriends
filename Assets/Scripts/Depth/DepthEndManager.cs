using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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

    [SerializeField] public float timeAnxiety = 0.1f;

    [SerializeField] private CanvasGroup black;


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
        AnxietyScript.instance.anxiety = PlayerPrefs.GetFloat("anxiety");
    }

    private IEnumerator protectionCheck() {
        if (eyeOpen) {
            if (!inProtection) {
                if(AnxietyScript.instance.anxiety < 0.99f) {
                    AnxietyScript.instance.addValueToNotKill(true, 0.005f);
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

        black.alpha = 1f;
        yield return new WaitForSeconds(0.25f);

        SceneManager.LoadScene("depthEndTalk");
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
