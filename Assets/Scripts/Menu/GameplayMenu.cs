using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayMenu : MonoBehaviour {

    public static GameplayMenu instance {  get; private set; }

    [SerializeField] private CanvasGroup whiteImage;

    [SerializeField] public float timeText = 5f;

    [SerializeField] private GameObject textField;
    [SerializeField] private List<string> textList = new List<string>();
    [SerializeField] public List<AudioClip> voice;

    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private float maxDistance = 7.5f;

    [SerializeField] private float timeTextFall = 2.0f;

    [SerializeField] private List<GameObject> plants = new List<GameObject>();

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    public void init() {
        StartCoroutine(startCount());
    }
    private IEnumerator startCount() {
        float elapsedTime = 0f;

        while (elapsedTime < 7.5f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for(int i = 0; i < textList.Count; i++) {
            elapsedTime = 0f;

            while (elapsedTime < timeText) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            awakeTextAppear(i);
        }        
        
        timeTextFall = 50f;
        for(int i = 0; i < 20; i++){
            elapsedTime = 0f;
            growFlower(i*2);
            while (elapsedTime < (1.0f) - 0.01f*i) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            awakeTextAppear(3);
            if(i == 15) {
                StartCoroutine(whiteAnim());
            }
        }
    }

    private IEnumerator whiteAnim() {
        whiteImage.alpha = 0;

        float elapsedTime = 0f;

        while (elapsedTime < 3.0f) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / 3.0f) * (elapsedTime / 3.0f));
            whiteImage.alpha = newAlpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        whiteImage.alpha = 1f;

        SceneManager.LoadScene("start");
    }

    private void growFlower(int num) {
        Vector3 position = GameObject.Find("Player").transform.position;
        position.x += Random.Range(-5, 5);
        position.z += Random.Range(-5, 5);
        position.y -= 5f;

        for(int i = 0; i < num; i++){
            GameObject plant = Instantiate(plants[Random.Range(0, plants.Count)],position, Quaternion.identity);
            plant.transform.Rotate(Vector3.left * 1f * Time.deltaTime);
            StartCoroutine(plantRise(plant));
        }
    }

    private IEnumerator plantRise(GameObject p) {
        float elapsedTime = 0f;
        float posAt = p.transform.position.y;
        float posTar = p.transform.position.y + 5f;

        while (elapsedTime < 1.5f) {
            p.transform.position = new Vector3(p.transform.position.x, Mathf.Lerp(posAt, posTar, (elapsedTime / 1.5f)), p.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private void awakeTextAppear(int text) {
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

        if(text == 3) {
            spawnPosition.y += (Random.value - 0.5f) * 7f;
            spawnPosition.x += (Random.value - 0.5f) * 7f;
            spawnPosition.z += (Random.value - 0.5f) * 7f;
        }

        if(spawnPosition.y < 0.5) {
            spawnPosition.y = 2f;
        }

        GameObject textObj = Instantiate(textField, spawnPosition, spawnRotation);
        textObj.GetComponent<TextMeshPro>().enabled = true;
        textObj.GetComponent<TextMeshPro>().fontSize = 8;
        textObj.GetComponent<TextScript>().DisplayText(textList[text]);
        StartCoroutine(makeTextFall(textObj));
    }

    private IEnumerator makeTextFall(GameObject textObj) {
        float elapsedTime = 0f;

        while (elapsedTime < timeTextFall) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textObj.GetComponent<Rigidbody>().useGravity = true;
        textObj.GetComponent<BoxCollider>().enabled = false;
        textObj.GetComponent<ImageScript>().enabled = false;
    }
}
