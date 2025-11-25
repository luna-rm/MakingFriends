using UnityEngine;

public class FallTrigger : MonoBehaviour { 
    private void OnTriggerEnter(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                DepthManager.instance.fall();
            }
        }
    }
}
