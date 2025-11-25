using UnityEngine;

public class fireflyTrigger : MonoBehaviour {

    [SerializeField] private GameObject firefly; 
    private void OnTriggerEnter(Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player" && firefly.GetComponent<fireflyScript>().startAnim != true) {
                firefly.GetComponent<fireflyScript>().startAnim = true;
            }
        }
    }
}
