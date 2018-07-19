using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDbox : DialogueBox {

	private EventQueue eq;
	private Vector3 posNormal = Vector3.zero;
	private Vector3 posChar = new Vector3(0, -200, 0);

	protected override void Start() {
		base.Start();
		eq = FindObjectOfType<EventQueue>();
	}

	protected override void Finish() {
		base.Finish();
		StartCoroutine("Dq");
	}

	private IEnumerator Dq() {
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		// Dequeue next event
		eq.Dequeue();
	}

	new public void ShowDialogue(string[] lines, Sprite[] faces) {
		base.ShowDialogue(lines, faces);
		if(faces[0] != null) {
			// If we start with a face, move the dbox down (assume characters are talking).
			transform.position = posChar;
		} else {
			transform.position = posNormal;
		}
	}
}
