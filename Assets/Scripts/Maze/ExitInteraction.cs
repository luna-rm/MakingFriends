using System.Collections;
using UnityEngine;

public class ExitInteraction : MonoBehaviour
{
    [SerializeField] private SpriteRenderer interaction;

    private bool enableInteraction = false;

    [SerializeField] private Animator trapdoor;

    [SerializeField] private GameObject trapdoorObject;

    [SerializeField] private ParticleSystem myParticleSystem;
    [SerializeField] private int burstAmount = 30;

    private bool alreadyTrigger = false;

    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                if(!alreadyTrigger) {
                    interaction.enabled = true;
                    enableInteraction = true;
                }
            }
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                interaction.enabled = false;
                enableInteraction = false;
            }
        }
    }

    private void Update() {
        if(enableInteraction && GameEventManager.InputContext == InputContextEnum.DEFAULT && !DialogueManager.alreadyStarted) {
            if (Input.GetKeyDown(KeyCode.E)){
                submitPressed();
            }
        }

        if (alreadyTrigger) {
            interaction.enabled = false;
            enableInteraction = false;
        }
    }

    private void submitPressed() {
        trapdoor.SetBool("open", true);
        alreadyTrigger = true;
        StartCoroutine(destroyTrap());
    }

    private IEnumerator destroyTrap() {
        yield return new WaitForSeconds(2.4f);
        myParticleSystem.Emit(burstAmount);
        yield return new WaitForSeconds(0.2f);
        Destroy(trapdoorObject);
    }
}
