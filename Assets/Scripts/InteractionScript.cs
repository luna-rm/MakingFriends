using UnityEngine;

public class InteractionScript : MonoBehaviour {

    [SerializeField] private SpriteRenderer interaction;
    [SerializeField] public string dialogue;

    private bool enableInteraction = false;

    public bool dialogueFinished = false;
    private bool isCurrentConversation = false; 

    // We use this flag to ensure we don't subscribe twice
    private bool isSubscribed = false;

    private void Start() {
        // Try to subscribe in Start if we haven't yet. 
        // Start runs after all Awakes, so GameEventManager is guaranteed to exist.
        SubscribeToEvents();
    }

    private void OnEnable() {
        // Try to subscribe when enabled (if GameEventManager is ready)
        SubscribeToEvents();
    }

    private void OnDisable() {
        if (GameEventManager.instance != null && isSubscribed) {
            GameEventManager.instance.dialogueEvents.onDialogueFinished -= OnDialogueFinished;
            isSubscribed = false;
        }
    }

    private void SubscribeToEvents() {
        // Only subscribe if the Manager exists and we haven't done it yet
        if (GameEventManager.instance != null && !isSubscribed) {
            GameEventManager.instance.dialogueEvents.onDialogueFinished += OnDialogueFinished;
            isSubscribed = true;
        }
    }

    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                interaction.enabled = true;
                enableInteraction = true;
                if(GameEventManager.InputContext == InputContextEnum.DEFAULT && DialogueManager.alreadyStarted && dialogue != null){
                    GameEventManager.InputContext = InputContextEnum.DIALOGUE;
                }
            }
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                interaction.enabled = false;
                enableInteraction = false;

                if(dialogue != null) {
                    if(GameEventManager.InputContext == InputContextEnum.DIALOGUE) {
                        GameEventManager.InputContext = InputContextEnum.DEFAULT;
                    }
                }
            }
        }
    }

    private void Update() {
        if(enableInteraction && GameEventManager.InputContext == InputContextEnum.DEFAULT && !DialogueManager.alreadyStarted) {
            if (Input.GetKeyDown(KeyCode.E)){
                submitPressed();
                interaction.enabled = false;
                enableInteraction = false;
            }
        }
    }

    private void submitPressed() {
        if(dialogue != null) {
            isCurrentConversation = true;
            
            // Optional: Reset the finished state if you want to detect it again
            dialogueFinished = false; 
            
            GameEventManager.instance.dialogueEvents.EnterDialogue(dialogue);
            GameEventManager.InputContext = InputContextEnum.DIALOGUE;
        }
    }

    private void OnDialogueFinished() {
        if(isCurrentConversation) {
            dialogueFinished = true;
            isCurrentConversation = false;
        }
    }
}