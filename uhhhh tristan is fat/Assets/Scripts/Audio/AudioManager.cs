using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static int number;
    public static AudioManager instance;

    public Sound[] sounds;
    private int index;

	// Use this for initialization
	void Awake () {

        /*
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
        */

        //DontDestroyOnLoad(gameObject);

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
        Debug.Log("Playing sound " + sound.name + " at volume " + sound.volume + ", source: " + sound.source.ToString());
        Debug.Log("Here's a random audio source component: " + GetComponent<AudioSource>());
        sound.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }
}
