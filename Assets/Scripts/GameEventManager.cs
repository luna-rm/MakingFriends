using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene events

public class GameEventManager : MonoBehaviour {
    public DialogueEvents dialogueEvents;
    
    public static GameEventManager instance { get; private set; }

    public static InputContextEnum InputContext { get; set; } = InputContextEnum.DEFAULT;
    public static WalkingContextEnum WalkingContext { get; set; } = WalkingContextEnum.WALKING;

    // Map variables
    public Coroutine MapCoroutine = null;
    public bool canOpenMap = false;
    [SerializeField] CanvasGroup map;
    [SerializeField] private float defaultFadeDuration = 0.5f;

    [SerializeField] private CanvasGroup fadeGroup;

    [SerializeField] private string goToDeath = "waters";
    
    // Persistent Data
    public int DeathCount = 0; // Replaced 'haveDied' with an integer counter

    private void Awake() {
        if(instance == null) {
            instance = this;
        } 

        dialogueEvents = new DialogueEvents();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        InputContext = InputContextEnum.DEFAULT;
        MapCoroutine = null;
        
        if(dialogueEvents != null) {
            dialogueEvents.isTyping = false;
        }
    }

    private void Update() {
        if (map != null && Input.GetKeyDown(KeyCode.M) && MapCoroutine == null && canOpenMap){
            if(map.alpha == 0f) {
                MapCoroutine = StartCoroutine(openMap());
            } else {
                MapCoroutine = StartCoroutine(closeMap());
            }
        }
    }

    public IEnumerator closeMap() {
        if (map == null) yield break;

        map.alpha = 1;
        float elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / defaultFadeDuration));
            if(map != null) map.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(map != null) map.alpha = 0f;
        MapCoroutine = null;
        InputContext = InputContextEnum.DEFAULT;
    }

    public IEnumerator openMap() {
        if (map == null) yield break; 

        InputContext = InputContextEnum.LOCKED;
        map.alpha = 0;
        float elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / defaultFadeDuration));
            if(map != null) map.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(map != null) map.alpha = 1f;
        MapCoroutine = null;
    }

    public IEnumerator appear() {
        InputContext = InputContextEnum.LOCKED;
        fadeGroup.alpha = 0;
        float elapsedTime = 0f;

        while (elapsedTime < 2.0f) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / 2.0f));
            fadeGroup.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeGroup.alpha = 1f;

        SceneManager.LoadScene(goToDeath);
    }

    public void die() {
        DeathCount++;
        StartCoroutine(appear());
    }
}