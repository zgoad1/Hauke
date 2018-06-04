using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeWhenFar : MonoBehaviour {

	[HideInInspector] public static List<FadeWhenFar> inactive = new List<FadeWhenFar>();
	public float distance;

	private Renderer r;
	private Color newColor;
	private FadeWhenClose fwc;

	// Use this for initialization
	void Start () {
		r = GetComponent<Renderer>();
		newColor = r.material.GetColor("_Color");   // only works for 2D sprite-based objects
		fwc = GetComponent<FadeWhenClose>();
	}
	
	// Update is called once per frame
	void Update () {

		newColor.a = Mathf.Clamp(distance / 10f - Vector3.Distance(transform.position, FadeFarController.cam.position) / 10f, 0, 1);
		if(fwc != null && fwc.visible || fwc == null) {
			r.material.SetColor("_Color", newColor);
		}
		if(newColor.a == 0) {
			gameObject.SetActive(false);
			inactive.Add(this);
		}
	}
}
