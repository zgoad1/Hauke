using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeWhenClose : MonoBehaviour {
	
	void Start () {
		if(GetComponent<Renderer>() != null) {
			GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
		}
		if(GetComponent<Rigidbody>() == null) {
			Rigidbody rb = gameObject.AddComponent<Rigidbody>();
			rb.useGravity = false;
			rb.isKinematic = true;
		}
	}

	public IEnumerator FadeOut(int frames) {
		StopCoroutine("FadeIn");
		Renderer r = gameObject.GetComponent<Renderer>();
		if(r != null) {
			// initial i corresponds to current alpha
			for(float i = (1 - r.material.color.a) * frames; i < frames; i++) {
				yield return null;
				r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 1 - i / frames);
			}
			r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0);
		}
	}

	public IEnumerator FadeIn(int frames) {
		StopCoroutine("FadeOut");
		Renderer r = GetComponent<Renderer>();
		if(r != null) {
			for(float i = r.material.color.a * frames; i < frames; i++) {
				yield return null;
				r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, i / frames);
			}
			r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 1);
		}
	}
}
