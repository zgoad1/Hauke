using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSound : EventItem {
	
	private AudioManager am;
	[SerializeField] private string sound;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		am = FindObjectOfType<AudioManager>();
		am.Play(sound);
		eq.Dequeue();
		Destroy(gameObject);
	}
}
