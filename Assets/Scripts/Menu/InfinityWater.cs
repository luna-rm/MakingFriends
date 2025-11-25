using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class InfinityWater : MonoBehaviour {

    [SerializeField] Material pixelate;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float defaultFadeDuration = 2.0f;
    [SerializeField] public List<AudioClip> waterSound = new List<AudioClip>();
    [SerializeField] public float timer = 1f;

    public static InfinityWater instance { get; private set; }
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    private void Update() {
        GameObject player = GameObject.Find("Player");

        if (player != null) {
            if(player.transform.position.z > 100 || player.transform.position.z < -100 || player.transform.position.x > 100 || player.transform.position.x < -100) {
                StartCoroutine(transitionAnim(player));        
            }
        }
    }

    public IEnumerator transitionAnim(GameObject player) {

        pixelate.SetInt("_Pixelation", 3);
        fadeCanvasGroup.alpha = 0;

        float elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration));
            fadeCanvasGroup.alpha = newAlpha;

            int newPixel = (int)Mathf.Lerp(3f, 100f, (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration));
            pixelate.SetInt("_Pixelation", newPixel);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        pixelate.SetInt("_Pixelation", 100);
        player.transform.position = Vector3.zero + new Vector3(0, 0.5f, 0);
        StartCoroutine(awakaningAnimation());
    }

    public IEnumerator awakaningAnimation() {
        pixelate.SetInt("_Pixelation", 100);
        fadeCanvasGroup.alpha = 1;

        float elapsedTime = 0f;

        while (elapsedTime < defaultFadeDuration) {
            float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration));
            fadeCanvasGroup.alpha = newAlpha;

            int newPixel = (int)Mathf.Lerp(100f, 3f, (elapsedTime / defaultFadeDuration) * (elapsedTime / defaultFadeDuration));
            pixelate.SetInt("_Pixelation", newPixel);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        pixelate.SetInt("_Pixelation", 3);
        GameEventManager.InputContext = InputContextEnum.DEFAULT;
    }
}
