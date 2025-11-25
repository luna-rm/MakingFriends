using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour {

    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    [SerializeField] private AnxietyScript anxietyScript;
    
    [SerializeField] private AudioSource heartBeatObject;
    [SerializeField] private AudioClip heartBeat1;
    [SerializeField] private AudioClip heartBeat2;
    [SerializeField] private AudioClip heartBeat3;

    private int anxietyLevel = 0;

    private void Awake() {
        if(instance == null) {
            instance = this;    
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume/4;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlaySoundFXClipPitch(AudioClip audioClip, Transform spawnTransform, float volume, Vector2 pit) {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume/4;
        audioSource.pitch = UnityEngine.Random.Range(pit.x, pit.y);
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    private void Update() {
        if (anxietyScript.anxiety >= 0f && anxietyScript.anxiety < 0.25f && anxietyLevel != 0) {
            heartBeatObject.Stop();
            anxietyLevel = 0;
        } else if (anxietyScript.anxiety >= 0.25f && anxietyScript.anxiety < 0.5f && anxietyLevel != 1) {
            heartBeatObject.clip = heartBeat1;
            heartBeatObject.volume = anxietyScript.anxiety * 0.5f;
            heartBeatObject.Play();
            anxietyLevel = 1;
        } else if (anxietyScript.anxiety >= 0.5f && anxietyScript.anxiety < 0.75f && anxietyLevel != 2) {
            heartBeatObject.clip = heartBeat2;
            heartBeatObject.volume = anxietyScript.anxiety * 0.5f;
            heartBeatObject.Play();
            anxietyLevel = 2;
        } else if (anxietyScript.anxiety >= 0.75f && anxietyScript.anxiety < 1f && anxietyLevel != 3) {
            heartBeatObject.clip = heartBeat3;
            heartBeatObject.volume = anxietyScript.anxiety * 0.5f;
            heartBeatObject.Play();
            anxietyLevel = 3;
        }        
    }
}
