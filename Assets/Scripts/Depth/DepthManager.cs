using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class DepthManager : MonoBehaviour {
    public static DepthManager instance { get; private set; }

    //Vector3(0,0,325.690002)

    public bool eyeOpen = true;
    public bool startEyeOpen = true;
    public bool inProtection = false;
    public bool startParticles = false;
    public Vector3 save = new Vector3(0,0,-6.4f);

    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject Camera;

    [SerializeField] private GameObject barrierObj;
    [SerializeField] private float speedBarrier = 0.5f;

    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> eyesToSpawn;
    [SerializeField] private int numberOfObjects = 100; 
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(320, 5, 150);

    [SerializeField] public float timeAnxiety = 0.2f;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } 
        PlayerPrefs.SetInt("Eye", 0);
    }

    private void Start() {
        GameEventManager.InputContext = InputContextEnum.LOCKED;
        Camera.transform.Rotate(-90f, 0, 0);
        StartCoroutine(initAnim());
    }

    private IEnumerator protectionCheck() {
        if (eyeOpen) {
            if (!inProtection) {
                AnxietyScript.instance.addValue(true, 0.001f);
                int e = PlayerPrefs.GetInt("Eye");
                e++;
                PlayerPrefs.SetInt("Eye", e);
                Debug.Log("Eye? " + e);
            }
        }

        yield return new WaitForSeconds(timeAnxiety);
        StartCoroutine(protectionCheck());
    }

    private IEnumerator initAnim() {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        Quaternion startRotation = Camera.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(90, 0f, 0);

        AnxietyScript.instance.anxiety = 0.1f;
        StartCoroutine(AnxietyScript.instance.textAppear(3));

        while (elapsedTime < 0.25f) {
            float t = elapsedTime / 0.25f;
            Camera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SpawnEyes();
        startEyeOpen = true;

        GameEventManager.InputContext = InputContextEnum.DEFAULT;
        StartCoroutine(protectionCheck());
    }

    public void fall() {
        blackScreen.alpha = 1.0f;
        player.transform.position = save;
        StartCoroutine(disappear());
    }

    public IEnumerator disappear() {
        float elapsedTime = 0f;

        while (elapsedTime < 1.0f) {
            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / 1.0f));
            blackScreen.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackScreen.alpha = 0f;
        AnxietyScript.instance.addValue(true, 0.1f);
    }

    private void Update() {
        if (eyeOpen) {
            float step = speedBarrier * Time.deltaTime;

            barrierObj.transform.position = Vector3.MoveTowards(barrierObj.transform.position, barrierObj.transform.position + new Vector3(0, 0, 350f), step);
        }
    }

    public void SpawnEyes() {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            
            // Spawn the object
            GameObject newObj = Instantiate(eyesToSpawn[Random.Range(0, eyesToSpawn.Count)], randomPos, Quaternion.identity);
            newObj.transform.localScale = Vector3.one * Random.Range(0.5f, 10f);

            // Parent it to this object to keep the scene organized
            newObj.transform.SetParent(this.transform);
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Calculate a random point inside the box defined by spawnAreaSize
        // Random.Range(-0.5f, 0.5f) centers the randomness around the object's position
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2) + player.transform.position.x;
        float y = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2) + 20f;
        float z = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2) + player.transform.position.z;

        Vector3 offset = new Vector3(x, y, z);

        // Add the offset to the spawner's current world position
        return transform.position + offset;
    }
}
