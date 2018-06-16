using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Sketchbook : MonoBehaviour {

	private PostProcessingProfile pp;
	[SerializeField] private int fadeFrames = 120;
	[SerializeField] private float minExposure = -3.75f;
	private float iPostExposure;
	private ColorGradingModel.Settings newSettings;
	private Renderer r;
	private EventQueue eq;

	void Reset() {
		pp = FindObjectOfType<MainCamera>().GetComponent<PostProcessingBehaviour>().profile;
		r = GetComponent<Renderer>();
		eq = FindObjectOfType<EventQueue>();
	}

	// Use this for initialization
	void Start() {
		Reset();
		iPostExposure = pp.colorGrading.settings.basic.postExposure;
		newSettings = pp.colorGrading.settings;
	}

	// Update is called once per frame
	void Update() {
	}

	public void ShowImage(Texture image) {
		IEnumerator cr = ChangeImage(image);
		StartCoroutine(cr);
	}

		private IEnumerator ChangeImage(Texture image) {
		StartCoroutine("FadeOut");
		for(int i = 0; i < fadeFrames; i++) yield return null;
		r.material.SetTexture("_MainTex", image);
		StartCoroutine("FadeIn");
	}

	private IEnumerator FadeOut() {
		for(int i = 0; i < fadeFrames; i++) {
			newSettings.basic.postExposure = iPostExposure - 0.5f * minExposure * (Mathf.Cos(2f * Mathf.PI / (2f * fadeFrames) * i) - 1);
			pp.colorGrading.settings = newSettings;
			yield return null;
		}
	}

	private IEnumerator FadeIn() {
		for(int i = 0; i < fadeFrames; i++) {
			newSettings.basic.postExposure = iPostExposure - 0.5f * minExposure * (Mathf.Cos(2f * Mathf.PI / (2f * fadeFrames) * (i + fadeFrames - 1)) - 1);
			pp.colorGrading.settings = newSettings;
			yield return null;
		}
		newSettings.basic.postExposure = iPostExposure;
		pp.colorGrading.settings = newSettings;
		if(eq != null) eq.Dequeue();
	}
}
