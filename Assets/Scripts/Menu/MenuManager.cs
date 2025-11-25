using System.Collections;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [SerializeField] private CanvasGroup init;
    [SerializeField] private RectTransform selector;
    private int option = 0;

    private int where = -1;

    [SerializeField] private CanvasGroup instr;

    [SerializeField] private CanvasGroup cred;

    [SerializeField] private float defaultFadeDuration = 2.0f;

    private void Start() {
        GameEventManager.InputContext = InputContextEnum.LOCKED;
    }

    void Update() {
        if (where == -1) {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)){
                submitSelection();
            } else if(Input.GetKeyUp(KeyCode.S)){
                option++;
                if(option > 2){
                    option = 0;
                }
                changePosSelection();
            } else if(Input.GetKeyUp(KeyCode.W)){
                option--;
                if(option < 0){
                    option = 2;
                }
                changePosSelection();
            }
        }

        if(where == 1) {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)){
                returnSelection();
            }
        }
    }

    private void changePosSelection() {
        if(option == 0){
            selector.anchoredPosition = new Vector2(-115f, -0f);
        } else if(option == 1){
            selector.anchoredPosition = new Vector2(-115f, -80f);
        } else if(option == 2){
            selector.anchoredPosition = new Vector2(-115f, -160f);
        }
    }

    private void submitSelection() {
        if(option == 0){
            initGame();
        } else if(option == 1){
            openInst();
        } else if(option == 2){
            openCred();
        }
    }

    private void initGame() {
        where = 2;
        StartCoroutine(disappear(init, null, 0));
    }

    private void openInst() {
        where = 2;
        StartCoroutine(disappear(init, instr, 1));
    }

    private void openCred() {
        where = 2;
        StartCoroutine(disappear(init, cred, 1));
    }

    private void returnSelection() {
        where = 2;
        if(option == 1) {
            StartCoroutine(disappear(instr, init, -1));
        } else if(option == 2){
            StartCoroutine(disappear(cred, init, -1));
        }
        option = 0;
        changePosSelection();
    }

    public IEnumerator disappear(CanvasGroup fadeGroup, CanvasGroup appearGroup, int toWhere) {
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
            StartCoroutine(appear(appearGroup, toWhere));
        } else {
            GameEventManager.InputContext = InputContextEnum.DEFAULT;
            GameplayMenu.instance.init();
        }
    }

    public IEnumerator appear(CanvasGroup appearGroup, int toWhere) {
        appearGroup.alpha = 0;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        where = toWhere;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / defaultFadeDuration));
            appearGroup.alpha = newAlpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        appearGroup.alpha = 1f;
    }
}
