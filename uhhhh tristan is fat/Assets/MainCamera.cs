using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider collision) {
		FadeWhenClose fader = collision.gameObject.GetComponent<FadeWhenClose>();
		if(fader != null) {
			fader.StartCoroutine("FadeOut", 8);
		}
	}

	private void OnTriggerExit(Collider collision) {
		FadeWhenClose fader = collision.gameObject.GetComponent<FadeWhenClose>();
		if(fader != null) {
			fader.StartCoroutine("FadeIn", 8);
		}
	}
}
