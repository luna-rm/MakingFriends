using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour {
    [SerializeField] string toScene = "start";
    [SerializeField] Vector3 toPos = Vector3.zero;

    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                GameObject.Find("FadeScreen").GetComponent<CanvasGroup>().alpha = 1;
                if(toScene != "") {
                    StartCoroutine(changeScene());
                } else {
                    StartCoroutine(changePos());
                }
            }
        }
    }

    private IEnumerator changeScene() {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(toScene);
    }

    private IEnumerator changePos() {
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("Player").transform.position = toPos;
    }
}
