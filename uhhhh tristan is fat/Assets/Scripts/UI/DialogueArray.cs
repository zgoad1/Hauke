using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueArray {
	public Sprite[] faces;
	public string[] text;

	public DialogueArray() {
		faces = new Sprite[1];
		text = new string[1];
	}

	public DialogueArray(Sprite[] f, string[] t) {
		faces = f;
		text = t;
	}

	public void Wow() {
		Debug.LogError("Wow");
	}
}
