using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchbookFadeImage : EventItem {

	private Sketchbook sb;
	[SerializeField] private Texture image;
	
	protected override void Start () {
		Debug.Log("Fading image");
		sb = FindObjectOfType<Sketchbook>();
		sb.ShowImage(image);
		Destroy(gameObject);
	}
}
