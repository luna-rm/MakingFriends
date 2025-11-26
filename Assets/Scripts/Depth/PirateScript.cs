using UnityEngine;

public class PirateScript : MonoBehaviour {
    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                StartCoroutine(DepthEndTalkManager.instance.lookToMe());
            }
        }
    }
}
