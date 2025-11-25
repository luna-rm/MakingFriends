using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextScript : MonoBehaviour {

    [SerializeField] private TextMeshPro dialogueText;
    [SerializeField] private float typingSpeed = 0.02f;

    private Coroutine typingCoroutine;
    private string currentSentence;
    private int enterCount = 0;    

    [SerializeField] private List<AudioClip> voice;
    private int voiceCount = 0;
    [SerializeField] private float volume = 0.5f;

    public bool isDialogue = true;

    private void Awake() {
        dialogueText.enabled = false;
        ResetPanel();
    }

    private void OnEnable() {
        if (isDialogue) {
            GameEventManager.instance.dialogueEvents.onDialogueStarted += DialogueStarted;
            GameEventManager.instance.dialogueEvents.onDialogueFinished += DialogueFinished;
            GameEventManager.instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;
            GameEventManager.instance.dialogueEvents.onSkipTyping += CompleteSentence;
        }
    }

    private void OnDisable() {
        if(isDialogue) {
            GameEventManager.instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
            GameEventManager.instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
            GameEventManager.instance.dialogueEvents.onDisplayDialogue -= DisplayDialogue;
            GameEventManager.instance.dialogueEvents.onSkipTyping -= CompleteSentence;
        }
    }

    private void DialogueStarted() {
        dialogueText.enabled = true;
    }

    private void DialogueFinished() {
        dialogueText.enabled = false;
        ResetPanel();
    }
    public void DisplayText(string textLine) {
        currentSentence = textLine;
        typingCoroutine = StartCoroutine(TypeSentence(textLine));
    }


    public void DisplayDialogue(string dialogueLine) {
        currentSentence = dialogueLine;
        
        // 1. Stop any existing typing
        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        ResetPanel(); 
        
        // 2. Start the new typing coroutine
        typingCoroutine = StartCoroutine(TypeSentence(dialogueLine));
    }

    private IEnumerator TypeSentence(string sentence) {
        GameEventManager.instance.dialogueEvents.isTyping = true;
        dialogueText.text = "";
        enterCount = 0;
        currentSentence = sentence;

        foreach (char letter in sentence.ToCharArray()) {
            if (enterCount >= 25 && letter.Equals(' ')) {
                dialogueText.text += "\n";
                enterCount = 0;
            } else {
                dialogueText.text += letter;
                enterCount++;
            }

            PlayVoice();
            yield return new WaitForSeconds(typingSpeed);
        }

        // Finished naturally
        GameEventManager.instance.dialogueEvents.isTyping = false;
        typingCoroutine = null;
    }

    private void CompleteSentence() {
        // FIX: We check 'typingCoroutine' specifically for THIS object.
        // This prevents other idle TextScripts from interfering with the active one.
        if (typingCoroutine != null) { 
            
            // 1. Stop the animation immediately
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            // 2. Tell the manager we are done
            GameEventManager.instance.dialogueEvents.isTyping = false;

            // 3. Rebuild the ENTIRE string from scratch instantly.
            // This guarantees the formatting (\n) is exactly the same as if it typed out.
            string finalBuiltText = "";
            int tempEnterCount = 0;

            foreach (char letter in currentSentence.ToCharArray()) {
                if(tempEnterCount >= 25 && letter.Equals(' ')) {
                    finalBuiltText += "\n";
                    tempEnterCount = 0;
                } else {
                    finalBuiltText += letter;
                    tempEnterCount++;
                }
            }

            // 4. Update the visual text
            dialogueText.text = finalBuiltText;
            
            // 5. Play sound
            PlayVoice();
        }
    }

    public void ResetPanel() {
        dialogueText.text =  "";
        currentSentence = "";
        enterCount = 0;
    }

    private void PlayVoice() {
        SoundFXManager.instance.PlaySoundFXClip(voice[voiceCount], transform, volume);
        voiceCount++;
        if(voiceCount >= voice.Count) { 
            voiceCount = 0;
        }
    }

    public string GetText() {
        return dialogueText.text;
    }
}
