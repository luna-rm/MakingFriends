using UnityEngine;

public class PirateScript : MonoBehaviour {
    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                DepthEndTalkManager.instance.StopCoroutine(DepthEndTalkManager.instance.animCoroutine);
                StartCoroutine(DepthEndTalkManager.instance.lookToMe());
                DepthEndTalkManager.instance.RedLight.intensity = 100;
            }
        }
    }
}
