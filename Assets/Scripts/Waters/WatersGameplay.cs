using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WatersGameplay : MonoBehaviour {
    [SerializeField] private CanvasGroup whiteImage;

    [SerializeField] public float timeText = 2.5f;

    [SerializeField] private GameObject textField;
    [SerializeField] private List<string> textList = new List<string>();
    [SerializeField] public List<AudioClip> voice;

    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private float maxDistance = 7.5f;

    [SerializeField] private float timeTextFall = 2.0f;

    public void Start() {
        StartCoroutine(startCount());
    }
    private IEnumerator startCount() {
        float elapsedTime = 0f;

        while (elapsedTime < 2.5f) {
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
        
        elapsedTime = 0f;

        while (elapsedTime < timeText) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(whiteAnim());
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

        SceneManager.LoadScene("startDied");
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
