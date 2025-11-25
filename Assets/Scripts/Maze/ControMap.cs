using UnityEngine;

public class ControMap : MonoBehaviour {
    [SerializeField] CanvasGroup map;
    [SerializeField] Transform player;
    [SerializeField] Camera mapCam;

    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float currentZoom = 40f; 
    [SerializeField] private float minZoom = 5f;      
    [SerializeField] private float maxZoom = 75f;    
    [SerializeField] private float scrollSensitivity = 10f;
    [SerializeField] private float panSpeed = 40f;

    private void Update() {
        if(map.alpha == 0f) {
            Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
            currentZoom = 40f;
            mapCam.orthographicSize = currentZoom;
        } else {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                currentZoom -= scroll * scrollSensitivity;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

                if (mapCam != null)
                {
                    mapCam.orthographicSize = currentZoom;
                }
            }

            float h = Input.GetAxisRaw("Horizontal"); 
            float v = Input.GetAxisRaw("Vertical");

            Vector3 move = new Vector3(h, 0, v).normalized * panSpeed * Time.unscaledDeltaTime;
            transform.Translate(move, Space.World);
        }
    }
}
