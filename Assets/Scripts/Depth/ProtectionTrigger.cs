using UnityEngine;

public class ProtectionTrigger : MonoBehaviour {
    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                DepthManager.instance.inProtection = true;
                DepthManager.instance.save = this.transform.position;
            }
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                DepthManager.instance.inProtection = false;
            }
        }
    }
}
