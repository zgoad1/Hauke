using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeWhenClose : MonoBehaviour {
	
	void Start () {
		if(GetComponent<Renderer>() != null) {
			foreach(Material m in GetComponent<Renderer>().materials) {
				m.SetInt("_ZWrite", 1);
			}
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
				foreach(Material m in r.materials) {
					m.color = new Color(m.color.r, m.color.g, m.color.b, 1 - i / frames);
				}
			}
			foreach(Material m in r.materials) {
				m.color = new Color(m.color.r, m.color.g, m.color.b, 0);
			}
		}
	}

	public IEnumerator FadeIn(int frames) {
		StopCoroutine("FadeOut");
		Renderer r = GetComponent<Renderer>();
		if(r != null) {
			for(float i = r.material.color.a * frames; i < frames; i++) {
				yield return null;
				foreach(Material m in r.materials) {
					m.color = new Color(m.color.r, m.color.g, m.color.b, i / frames);
				}
			}
			foreach(Material m in r.materials) {
				m.color = new Color(m.color.r, m.color.g, m.color.b, 1);
			}
		}
	}
}
