using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    private List<AudioClip> waterSound = new List<AudioClip>();
    private float timer = 1f;
    private bool cd = true;

    private void Start() {
        waterSound = InfinityWater.instance.waterSound;
        timer = InfinityWater.instance.timer;
    }

    private void OnCollisionStay(Collision collision) {
        if(collision != null) {
            if(collision.gameObject.name == "Player") {
                if(cd) {
                    if(collision.gameObject.GetComponent<Player>().moveForward > 0 || collision.gameObject.GetComponent<Player>().moveHorizontal > 0) {
                        SoundFXManager.instance.PlaySoundFXClipPitch(waterSound[(int)Random.Range(0, waterSound.Count)], collision.gameObject.transform, 0.2f,  new Vector2(0.7f, 1));
                        cd = false;
                        StartCoroutine(cdTimer());
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator cdTimer() {
        float elapsedTime = 0f;

        float auxTimer = timer;

        if(GameEventManager.WalkingContext == WalkingContextEnum.SPRINTING) {
            auxTimer = timer / 1.5f;
        }

        while (elapsedTime < timer) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cd = true;
    }
}
