using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NPC : Interactable {

	protected Animator anim;

	new protected void Reset() {
		anim = GetComponent<Animator>();
		base.Reset();
	}

	public override void Interact() {
		Debug.Log("Attempting to show dialogue");
		dbox.ShowDialogue(dialogue[ttt].text, dialogue[ttt].faces);
		ttt++;
	}
}
