using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour {

    [SerializeField] private string goTo = "startEnd";

    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                PlayerPrefs.SetFloat("anxiety", AnxietyScript.instance.anxiety);
                SceneManager.LoadScene(goTo);
            }
        }
    }
}
