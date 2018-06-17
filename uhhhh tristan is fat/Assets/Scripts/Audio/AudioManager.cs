using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private static int number;
    private static AudioManager instance;

    [SerializeField] private Sound[] sounds;
    private int index;

	// Use this for initialization
	void Awake () {

        index = number;
        number++;
        if(instance == null) {
            instance = this;
            Debug.Log("Creating audio manager #" + index);
        } else {
            Destroy(gameObject);
            Debug.Log("Destroying extra audio manager #" + index);
            return;
        }
		
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
	}

    public void Play(string name) {
        Sound sound = Array.Find(sounds, s => s.name == name);
        sound.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
	}

	IEnumerator FadeInTrack(string name, int fadeFrames) {
		Sound sound = Array.Find(sounds, s => s.name == name);
		for(float i = 0; i < fadeFrames; i++) {
			sound.source.volume = (i + 1) / fadeFrames * sound.volume;
			yield return null;
		}
	}

	IEnumerator FadeOutTrack(string name, int fadeFrames) {
		Sound sound = Array.Find(sounds, s => s.name == name);
		for(float i = 0; i < fadeFrames; i++) {
			sound.source.volume = (fadeFrames - i) / fadeFrames * sound.volume;
			yield return null;
		}
		Stop(name);
		//Debug.Log("Faded out " + name);
	}

	public void FadeIn(string name, int fadeFrames) {
		IEnumerator cr = FadeInTrack(name, fadeFrames);
		StartCoroutine(cr);
	}

	public void FadeOut(string name, int fadeFrames) {
		IEnumerator cr = FadeOutTrack(name, fadeFrames);
		StartCoroutine(cr);
	}
}
