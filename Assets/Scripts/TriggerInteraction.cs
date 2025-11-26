using UnityEngine;

public class TriggerInteraction : MonoBehaviour {
    [SerializeField] private SpriteRenderer interaction;
    [SerializeField] public string dialogue;

    private bool enableInteraction = false;

    public bool dialogueFinished = false;
    private bool isCurrentConversation = false; 

    private bool isSubscribed = false;

    private void Start() {
        SubscribeToEvents();
    }

    private void OnEnable() {
        SubscribeToEvents();
    }

    private void OnDisable() {
        if (GameEventManager.instance != null && isSubscribed) {
            GameEventManager.instance.dialogueEvents.onDialogueFinished -= OnDialogueFinished;
            isSubscribed = false;
        }
    }

    private void SubscribeToEvents() {
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

                submitPressed();
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
