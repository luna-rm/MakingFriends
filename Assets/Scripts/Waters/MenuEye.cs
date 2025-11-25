using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEye : MonoBehaviour {

    [SerializeField] private CanvasGroup eye;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private AudioSource sound;
    
    private bool anim = false;

    void Update() {
        if(!anim) {
            if(this.GetComponent<CanvasGroup>().alpha == 1) {
                eye.alpha = 1;
                //add animation
                anim = true;
                sound.volume = 0.666f;
                StartCoroutine(appear());
            }
        }
    }

    public IEnumerator appear() {
        background.alpha = 0;

        float elapsedTime = 0f;

        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < 1) {
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime / 1));
            float newVolume = Mathf.Lerp(0.666f, 0.222f, (elapsedTime / 1));
            background.alpha = newAlpha;
            sound.volume = newVolume;

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        SceneManager.LoadScene("depth");
    }
}
