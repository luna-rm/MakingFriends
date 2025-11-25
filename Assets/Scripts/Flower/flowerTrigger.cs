using System.Collections;
using UnityEngine;

public class flowerTrigger : MonoBehaviour {
    [SerializeField] private GameObject flower; 
    [SerializeField] private ParticleSystem myParticleSystem;
    [SerializeField] private float anxietyUp = 0.1f;

    private void OnTriggerEnter(Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                myParticleSystem.Emit(8);
                GameObject.Find("Global Volume").GetComponent<AnxietyScript>().addValue(true, anxietyUp);
                StartCoroutine(destroyFlower());
            }
        }
    }

    private IEnumerator destroyFlower() {
        yield return new WaitForSeconds(0.2f);
        Destroy(flower);
   }
}
