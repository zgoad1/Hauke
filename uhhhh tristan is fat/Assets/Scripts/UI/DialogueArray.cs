using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueArray {
	//public Sprite[] faces;
	//public string[] text;
	public FaceText[] items;
	public GameObject _event;

	public DialogueArray() {
		items = new FaceText[0];
		/*
		faces = new Sprite[1];
		text = new string[1];
		*/
	}

	/*
	public DialogueArray(Sprite[] f, string[] t) {
		faces = f;
		text = t;
	}
	*/

	public DialogueArray(FaceText[] i) {
		items = i;
	}
}
