using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndFlower : MonoBehaviour {
    [SerializeField] GameObject Player;

    [SerializeField] GameObject flower0;
    [SerializeField] private TextScript flower0Text;

    private bool drawAnim = false;
    [SerializeField] GameObject draw1;
    [SerializeField] GameObject draw2;

    private bool endAnim = false;

    private InteractionScript flower0Interaction;

    [SerializeField] private CanvasGroup fadeGroup;

    private void Awake() {
        flower0Interaction = flower0.GetComponent<InteractionScript>();
    }

    private void Update() {
        if (!drawAnim) {
            if(flower0Text.GetText() == "Deixa eu ver os desenhos!!") {
                drawAnim = true;
                draw1.GetComponent<DrawScript>().go = true;
                draw1.SetActive(true);
                draw2.GetComponent<DrawScript>().go = true;
                draw2.SetActive(true);
            }
        }

        if (!endAnim) {
            if (flower0Interaction.dialogueFinished) {
                StartCoroutine(goToNext());
            }
        }
    }

    public IEnumerator goToNext() {
        GameEventManager.InputContext = InputContextEnum.LOCKED;
        fadeGroup.alpha = 0;
        float elapsedTime = 0f;

        while (elapsedTime < 2.0f) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / 2.0f));
            fadeGroup.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeGroup.alpha = 1f;

        SceneManager.LoadScene("toDepth");
    }
}
