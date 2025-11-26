using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DepthEndTalkManager : MonoBehaviour {

    public static DepthEndTalkManager instance { get; private set; }

    public bool eyeOpen = true;
    public bool startEyeOpen = true;
    public bool inProtection = false;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject Camera;

    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> eyesToSpawn; 
    [SerializeField] private int numberOfObjects = 200;
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

    public bool needLock = true;

    [SerializeField] float waitInit = 10f;

    [SerializeField] public Light RedLight;

    public Coroutine animCoroutine = null;

    [SerializeField] TextScriptPirate text;

    [SerializeField] DrawScript fish0;
    [SerializeField] DrawScript fish1;

    private bool endAnim = false;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } 
    }

    private void Start() {
        RedLight.intensity = 1;

        SpawnEyes();
        GameEventManager.InputContext = InputContextEnum.LOCKED;
        CrySounds.Play();

        postProcessing.profile.TryGet(out UnityEngine.Rendering.Universal.DepthOfField df);
        depthOfField = df;
        depthOfField.active = true;
        depthOfField.gaussianStart.value = 0;

        //var cEmission = cry.emission;
        //cEmission.enabled  = false;
        //cEmission = cry1.emission;
        //cEmission.enabled  = false;

        if (!animStarted) {
            animCoroutine = StartCoroutine(anim());
            animStarted = true;
        }

        player.GetComponent<Player>().canMove = false;
    }

    private IEnumerator anim() {
        yield return new WaitForSeconds(waitInit);

        float elapsedTime = 0f;

        float startZ = Pirate.transform.position.z;
        float startX = Pirate.transform.position.x;

        while (elapsedTime < 7f) {
            float newX = Mathf.Lerp(startX, -3.5f, (elapsedTime / 7f));
            Pirate.transform.position = new Vector3(newX, Pirate.transform.position.y, Pirate.transform.position.z);

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

    private void Update() {
        if(GameEventManager.InputContext != InputContextEnum.LOCKED && needLock) {
            GameEventManager.InputContext = InputContextEnum.LOCKED;
        }

        if (text.GetText() == "Deixa eu ver!!"){
            fish0.gameObject.SetActive(true);
            fish0.go = true;

            fish1.gameObject.SetActive(true);
            fish1.go = true;
        }

        if (Pirate.GetComponent<TriggerInteraction>().dialogueFinished && !endAnim){
            endAnim = true;
            StartCoroutine(endScene());
        }
    }

    public IEnumerator lookToMe() {
        float elapsedTime = 0f;
        
        Quaternion startRotation = player.transform.rotation;

        Vector3 directionToTarget = Pirate.transform.position - player.transform.position;
        directionToTarget.y = 0;
        Quaternion endRotation = Quaternion.LookRotation(directionToTarget);

        while (elapsedTime < 2f) {
            float linearT = elapsedTime / 2f;

            float t = Mathf.Sin(linearT * Mathf.PI * 0.5f);

            player.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            RedLight.intensity = Mathf.Lerp(100f, 0, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.rotation = endRotation;

        StartCoroutine(ReduceAnxiety());
    }

    private IEnumerator ReduceAnxiety(){
        startEyeOpen = false;
        float elapsedTime = 0f;

        cry.gameObject.SetActive(false);
        cry1.gameObject.SetActive(false);

        while (elapsedTime < 7f){
            float newA = Mathf.Lerp(0.99f, 0f, (elapsedTime / 7f));
            AnxietyScript.instance.anxiety = newA;

            float newV = Mathf.Lerp(0.2f, 0f, (elapsedTime / 7f));
            CrySounds.volume = newV;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator endScene() {
        float elapsedTime = 0f;

        while (elapsedTime < 5f){
            float newA = Mathf.Lerp(0f, 1f, (elapsedTime / 5f));
            black.alpha = newA;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("end");
    }
}
