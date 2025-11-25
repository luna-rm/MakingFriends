using UnityEngine;

public class fireflyScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float verticalSpeed = 0.5f; 
    [SerializeField] private float horizontalSpeed = 1.0f; 
    [SerializeField] private float wobbleAmount = 0.5f; 

    [Header("Life Cycle")]
    [SerializeField] private float maxHeight = 10f; 
    [SerializeField] private bool respawnAtBottom = true; 

    private float randomOffset;
    private Vector3 startPosition;

    public bool startAnim = false;

    void Start() {
        randomOffset = Random.Range(0f, 100f);
        startPosition = transform.position += new Vector3(Random.Range(-2f, 2f), -0f, Random.Range(-2f, 2f));
        transform.position = startPosition;
    }

    void Update() {
        if (startAnim) {
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;

            float noiseX = Mathf.PerlinNoise(Time.time * horizontalSpeed, randomOffset) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.time * horizontalSpeed, randomOffset + 50f) - 0.5f;

            Vector3 wobble = new Vector3(noiseX, 0, noiseZ) * wobbleAmount * Time.deltaTime;
            transform.position += wobble;

            if (transform.position.y > startPosition.y + maxHeight) {
                if (respawnAtBottom) {
                    ResetPosition();
                } else {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void ResetPosition() {
        Vector3 resetPos = transform.position;
        resetPos.y = startPosition.y;
        transform.position = resetPos;
    }
}