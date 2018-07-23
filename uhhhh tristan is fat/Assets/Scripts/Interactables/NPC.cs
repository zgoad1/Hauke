using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NPC : Interactable {

	protected Animator anim;
	[HideInInspector] public NPCHead head;
	protected Quaternion irot;

	new protected void Reset() {
		anim = GetComponent<Animator>();
		Transform h = Array.Find(transform.GetComponentsInChildren<Transform>(), t => t.gameObject.name == "Head");
		if(h.GetComponent<NPCHead>() == null) head = h.gameObject.AddComponent<NPCHead>();
		else head = h.GetComponent<NPCHead>();
		head.body = transform;
		Debug.Log("setting head.body to: " + head.body);
		irot = transform.localRotation;
		base.Reset();
	}

	new protected void Start() {
		Reset();
		base.Start();
	}

	public override void Interact() {
		if(dialogue[ttt].items.Length != 0) {
			player.head.FaceTransform(head.transform);
			dbox.heads.Add(player.head);
			if(dialogue[ttt].turnBody) {
				TurnBody(player.transform);
				dbox.bodies.Add(this);
			}
			if(dialogue[ttt].turnHead) {
				head.FaceTransform(player.head.transform);
				dbox.heads.Add(head);
			}
			dbox.ShowDialogue(dialogue[ttt].items);
			ttt++;
		} else if(dialogue[ttt]._event != null) {
			Instantiate(dialogue[ttt]._event);
			player.Pause();
			Debug.Log("Starting event");
		}
	}

	// Turn body to pre-interaction rotation
	public void TurnBack() {
		if(dialogue[ttt - 1].turnBody) SmoothTurn(irot);	// ttt increments immediately as you interact; the interaction in question will be the "previous" one
	}

	// SmoothTurn the body along the Y axis to face a target
	private void TurnBody(Transform target) {
		Quaternion oldRot = transform.localRotation;
		MTSBBI.LookAtXYZ(transform, target.transform, 2, 1);
		Quaternion newRot = transform.localRotation;
		transform.rotation = oldRot;
		SmoothTurn(newRot);
	}

	public void SmoothTurn(Quaternion q) {
		IEnumerator cr = Controllable.SmoothTurnCR(transform, q, 0.2f);
		StartCoroutine(cr);
	}
}
