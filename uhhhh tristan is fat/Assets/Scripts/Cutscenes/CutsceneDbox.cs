using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDbox : DialogueBox {

	private EventQueue eq;

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
}
