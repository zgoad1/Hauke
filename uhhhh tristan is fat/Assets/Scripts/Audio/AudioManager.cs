using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField] private Sound[] sounds;
	private static int amount = 0;
	public int number;

	// Use this for initialization
	void Start () {
		number = amount;
		amount++;
		
        if(instance == null) {
            instance = this;
            Debug.Log("Creating audio manager");

			DontDestroyOnLoad(gameObject);

			foreach(Sound s in sounds) {
				s.source = gameObject.AddComponent<AudioSource>();
				Debug.LogWarning("Setting sound source to: " + s.source);
				s.source.clip = s.clip;
				s.source.volume = s.volume;
				s.source.pitch = s.pitch;
				s.source.loop = s.loop;
			}
		} else {
            Destroy(gameObject);
            Debug.Log("Destroying extra audio manager");
            return;
		}
		PrintSources();
	}

	private void Update() {
		Debug.Log("Audio manager #" + number + " exists.");
	}

	public void Play(string name) {
		PrintSources("PLAY:");
		Sound sound = Array.Find(sounds, s => s.name == name);
		if(sound != null) {
			Debug.Log("Playing sound: " + sound.name + "\nSource: " + sound.source);
			sound.source.Play();
		} else {
			Debug.LogError("AUDIO: could not find sound: " + name + "\nProbably because you aren't using the first audio manager in the Sketchbook scene\naka this isn't a bug");
		}
	}

    public void Stop(string name) {
		PrintSources("STOP:");
		Sound sound = Array.Find(sounds, s => s.name == name);
		if(sound != null) {
			sound.source.Stop();
		} else {
			Debug.LogError("AUDIO: could not find sound: " + name + "\nProbably because you aren't using the first audio manager in the Sketchbook scene\naka this isn't a bug");
		}
	}

	IEnumerator FadeInTrack(string name, int fadeFrames) {
		Sound sound = Array.Find(sounds, s => s.name == name);
		for(float i = 0; i < fadeFrames; i++) {
			sound.source.volume = (i + 1) / fadeFrames * sound.volume;
			yield return null;
		}
		PrintSources();
	}

	IEnumerator FadeOutTrack(string name, int fadeFrames) {
		Sound sound = Array.Find(sounds, s => s.name == name);
		for(float i = 0; i < fadeFrames; i++) {
			sound.source.volume = (fadeFrames - i) / fadeFrames * sound.volume;
			yield return null;
		}
		Stop(name);
		PrintSources();
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

	private void PrintSources(string str) {
		Debug.Log(str + "(#" + number + ")");
		foreach(Sound s in sounds) {
			Debug.Log("SOUND: " + s.name + "\nSOURCE: " + s.source);
		}
	}
	private void PrintSources() {
		PrintSources("");
	}
}
