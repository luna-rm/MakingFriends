using UnityEngine;
using Ink.Runtime;
using System.Collections;

public class DialogueManager : MonoBehaviour {

    public static bool dialoguePlaying = false;
    public static bool alreadyStarted = false;

    [SerializeField] private TextAsset inkJson;
    private Story story;

    private void Awake() {
        story = new Story(inkJson.text);
    }

    private void OnEnable() {
        GameEventManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
    }

    private void OnDisable() {
        GameEventManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
    }

    private void EnterDialogue(string dialogueName) {
        if(dialoguePlaying) {
            return;
        }
        dialoguePlaying = true;
        alreadyStarted = true;

        story.ChoosePathString(dialogueName);

        GameEventManager.instance.dialogueEvents.DialogueStarted();

        ContinueOrExitStory();
    }

    private void Update() {
        if(dialoguePlaying) {
            if (Input.GetKeyDown(KeyCode.E) && GameEventManager.InputContext == InputContextEnum.DIALOGUE){
                if (GameEventManager.instance.dialogueEvents.isTyping) {
                    GameEventManager.instance.dialogueEvents.SkipTyping();
                } else {
                    ContinueOrExitStory();
                }
            }
        }
        else if(alreadyStarted){
            if (story.canContinue) {
                ExitDialogue();
            }
        }
    }

    private void ContinueOrExitStory() {
        if (story.canContinue) {
            string dialogueLine = story.Continue();
            GameEventManager.instance.dialogueEvents.DisplayDialogue(dialogueLine);
        } else {
            StartCoroutine(ExitDialogue());
        }
    }

    private IEnumerator ExitDialogue() {  

        yield return null;

        dialoguePlaying = false;
        alreadyStarted = false;
        GameEventManager.InputContext = InputContextEnum.DEFAULT;
        GameEventManager.instance.dialogueEvents.DialogueFinished();
        story.ResetState();
    }
}
