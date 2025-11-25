using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toDepth : MonoBehaviour {

    [SerializeField] List<CanvasGroup> thoughts;
    private int thoughtNum = -1;

    [SerializeField] float defaultFadeDuration = 1.0f;

    private Coroutine inCoroutine = null;

    void Start() {
        inCoroutine = StartCoroutine(init());
        GameEventManager.InputContext = InputContextEnum.LOCKED;
    }

    private IEnumerator init() {
        yield return new WaitForSeconds(2.0f);
        inCoroutine = StartCoroutine(appear(thoughts[0]));
        GameEventManager.InputContext = InputContextEnum.LOCKED;
    }

    private void Update() {
        if(inCoroutine == null && thoughtNum < thoughts.Count-1) {
            if (Input.GetKeyDown(KeyCode.E)){
                submitPressed();
            }
        }
    }

    private void submitPressed() {
        inCoroutine = StartCoroutine(disappear(thoughts[thoughtNum], thoughts[thoughtNum+1]));
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
            inCoroutine = StartCoroutine(appear(appearGroup));
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
        thoughtNum++;
        inCoroutine = null;
    }
}
