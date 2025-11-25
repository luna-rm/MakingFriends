using UnityEngine;

public class PlaceMap : MonoBehaviour {
    [SerializeField] Transform player;

    void Update() {
        transform.localPosition = new Vector3(player.position.x, 75, player.position.z);
    }
}
