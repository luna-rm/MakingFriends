using System.Collections;
using UnityEngine;

public class EndManager : MonoBehaviour {
    [SerializeField] private CanvasGroup cred;
    [SerializeField] private CanvasGroup obg;
    [SerializeField] private CanvasGroup points;
    [SerializeField] private CanvasGroup photo;

    int where = 0;

    [SerializeField] private float defaultFadeDuration = 1.0f;

    private void Start() {
        GameEventManager.InputContext = InputContextEnum.LOCKED;
        StartCoroutine(appear(cred));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)){
            submitSelection();
        }
    }

    private void submitSelection() {
        if(where == 0){
            where++;
            StartCoroutine(disappear(cred, obg));
        } else if(where == 1){
            where++;
            StartCoroutine(disappear(obg, points));
        } else if(where == 2){
            where++;
            StartCoroutine(disappear(points, null));
            //StartCoroutine(disappear(points, photo));
        } else if(where == 3){
            where++;
            StartCoroutine(disappear(photo, null));
        }
    }

    public IEnumerator disappear(CanvasGroup fadeGroup, CanvasGroup appearGroup) {
        fadeGroup.alpha = 1;

        float elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / defaultFadeDuration));
            fadeGroup.alpha = newAlpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeGroup.alpha = 0f;

        if(appearGroup != null){
            StartCoroutine(appear(appearGroup));
        } else {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public IEnumerator appear(CanvasGroup appearGroup) {
        appearGroup.alpha = 0;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / defaultFadeDuration));
            appearGroup.alpha = newAlpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        appearGroup.alpha = 1f;
    }
}
