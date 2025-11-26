using System.Collections;
using UnityEngine;

public class EyeScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float time;

    private Coroutine at;

    private void Start() {
        time = Random.Range(time - 1f, time + 1f);
        Color tempColor = sr.color;
        tempColor.a = 0f;
        sr.color = tempColor; 
    }

    private void Update() {
        if(DepthManager.instance != null) {
            if (DepthManager.instance.startEyeOpen) {
                at = StartCoroutine(appear());
            } else {
                at = StartCoroutine(disappear());
            }
        } else if(DepthEndManager.instance != null){
            if (DepthEndManager.instance.startEyeOpen) {
                at = StartCoroutine(appear());
            } else {
                at = StartCoroutine(disappear());
            }  
        } else if(DepthEndTalkManager.instance != null){
            if (DepthEndTalkManager.instance.startEyeOpen) {
                at = StartCoroutine(appear());
            } else {
                at = StartCoroutine(disappear());
            }  
        }        
    }

    public IEnumerator appear() {
        float elapsedTime = 0f;
        
        Color tempColor = sr.color;
        while (elapsedTime < time) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / time));
            tempColor.a = newAlpha;
            sr.color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        at = null;
    }

    public IEnumerator disappear() {
        //yield return new WaitForSeconds(time);
        float elapsedTime = 0f;

        Color tempColor = sr.color;
        while (elapsedTime < time/2) {
            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / (time/2)));
            tempColor.a = newAlpha;
            sr.color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        at = null;

        Destroy(gameObject);
    }
}
