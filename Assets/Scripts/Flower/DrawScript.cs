using UnityEngine;

public class DrawScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 target; // Drag the destination object here
    [SerializeField] private float speed = 5f; // Movement speed
    [SerializeField] private float stoppingDistance = 0.1f; // How close before stopping

    public bool go = false;

    private void Update() {
        if (go) {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            float distance = Vector3.Distance(transform.position, target);

            if (distance > stoppingDistance) {
                float step = speed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, target, step);
            }
        } else {
            transform.position = GameObject.Find("Player").transform.position;
        }
    }
    
}
