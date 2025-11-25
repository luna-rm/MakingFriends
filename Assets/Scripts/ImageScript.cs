using System.Collections;
using UnityEngine;

public class ImageScript : MonoBehaviour {

    private Camera m_Camera;

    private bool inAnimation = false;
    public float animTime = 0.5f; 

    private void Start() {
        m_Camera = Camera.main;
    }

    void Update() { 
        if (!inAnimation) {
            Vector3 cameraPos = m_Camera.transform.position;

            transform.LookAt(cameraPos);
            transform.Rotate(0f, 180f, 0f);
        }
    }

    public IEnumerator disappear() {
        inAnimation = true;

        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        // Calculate where we want to end up (current + 180 degrees on Y axis)
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 90f, 0);

        while (elapsedTime < animTime) {
            // Calculate percentage (0.0 to 1.0)
            float t = elapsedTime / animTime;
        
            // Smoothly interpolate between start and end
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Force it to be exactly 180 at the end to fix any tiny remainders
        transform.rotation = targetRotation;
        this.gameObject.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public IEnumerator appear() {
        inAnimation = true;
        this.gameObject.gameObject.GetComponent<SpriteRenderer>().enabled = true;

        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        // Calculate where we want to end up (current + 180 degrees on Y axis)
        Quaternion targetRotation = startRotation;

        startRotation *= Quaternion.Euler(0, 90f, 0);

        while (elapsedTime < animTime) {
            // Calculate percentage (0.0 to 1.0)
            float t = elapsedTime / animTime;
        
            // Smoothly interpolate between start and end
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Force it to be exactly 180 at the end to fix any tiny remainders
        transform.rotation = targetRotation;
        inAnimation = false;
    }
}
