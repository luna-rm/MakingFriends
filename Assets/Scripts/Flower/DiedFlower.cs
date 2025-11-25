using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiedFlower : MonoBehaviour {
    [SerializeField] GameObject flower0;
    [SerializeField] private TextScript flower0Text;

    private bool anim = false;

    private InteractionScript flower0Interaction;
    private ImageScript flower0Image;

    private void Awake() {
        flower0Interaction = flower0.GetComponent<InteractionScript>();
        flower0Image = flower0.GetComponent<ImageScript>();
    }

    private void Update() {
        if (!anim) {
            if(flower0Interaction.dialogueFinished || GameEventManager.instance.canOpenMap) {
                anim = true;
                StartCoroutine(anim0());
            }
        }
    }
    private IEnumerator anim0() {
        StartCoroutine(flower0Image.disappear());
        flower0Text.enabled = false;
        yield return new WaitForSeconds(flower0Image.animTime + 0.2f);

        flower0.SetActive(false);
    }

}
