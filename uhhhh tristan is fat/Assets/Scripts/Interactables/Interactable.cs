using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	protected SphereCollider sphere;
	[SerializeField] protected float radius = 10;
	protected Controllable player;
	protected DialogueBox dbox;
	protected CutsceneDbox dboxcs;
	protected int t = 0;
	protected int ttt {
		get {
			return t;
		}
		set {
			t = Mathf.Min(dialogue.Length - 1, value);
		}
	}

	public DialogueArray[] dialogue = new DialogueArray[2];

	// Use this for initialization
	protected void Start () {
		player = FindObjectOfType<Controllable>();
		DialogueBox[] dboxes = FindObjectsOfType<DialogueBox>();
		dbox = Array.Find(dboxes, db => db.name == "Dbox"); // We probably have "Dbox" and "Cutscene Dbox"
		dboxcs = (CutsceneDbox)Array.Find(dboxes, db => db.name == "Cutscene Dbox");
	}
	
	// Update is called once per frame
	protected void Update () {

	}

	protected void Reset() {
		sphere = GetComponent<SphereCollider>();
		sphere.isTrigger = true;
		sphere.radius = radius;
		sphere.center = new Vector3(0, 8, 0);
	}

	// Add to player's list of close NPCs
	private void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Controllable>() != null) {
			player.AddInteractable(this);
		}
	}

	// Remove from said list
	private void OnTriggerExit(Collider other) {
		if(other.GetComponent<Controllable>() != null) {
			player.RemoveInteractable(this);
		}
	}

	public virtual void Interact() {

	}
}
