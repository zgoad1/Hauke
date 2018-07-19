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
	protected Quaternion irot;

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
		if(dialogue[0].text.Length != 0) {
			irot = transform.localRotation;
			player.head.FaceTransform(head.transform);
			dbox.heads.Add(player.head);
			if(turnBody) {
				TurnBody(player.transform);
				dbox.bodies.Add(this);
			}
			if(turnHead) {
				head.FaceTransform(player.head.transform);
				dbox.heads.Add(head);
			}
			dbox.ShowDialogue(dialogue[ttt].text, dialogue[ttt].faces);
			ttt++;
		} else if(dialogue[0]._event != null) {
			Instantiate(dialogue[0]._event);
		}
	}

	// Turn body to pre-interaction rotation
	public void TurnBack() {
		if(turnBody) SmoothTurn(irot);
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
