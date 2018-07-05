using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NPC : Interactable {

	protected Animator anim;
	[HideInInspector] public NPCHead head;
	[SerializeField] protected bool turnBody = false;
	[SerializeField] protected bool turnHead = true;

	new protected void Reset() {
		anim = GetComponent<Animator>();
		Transform h = Array.Find(transform.GetComponentsInChildren<Transform>(), t => t.gameObject.name == "Head");
		head = h.gameObject.AddComponent<NPCHead>();
		base.Reset();
	}

	new protected void Start() {
		Reset();
		base.Start();
	}

	public override void Interact() {
		player.FaceTransform(true, head.transform);
		if(turnHead) head.SetFacing(player.head);
		dbox.ShowDialogue(dialogue[ttt].text, dialogue[ttt].faces);
		ttt++;
	}

	public void SmoothTurn(Quaternion q) {
		IEnumerator cr = Controllable.SmoothTurnCR(transform, q, 0.2f);
		StartCoroutine(cr);
	}
}
