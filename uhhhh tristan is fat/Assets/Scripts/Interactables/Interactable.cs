using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	protected SphereCollider sphere;
	[SerializeField] protected float radius = 10;
	public bool zoomIn = true;
	protected Controllable player;
	protected DialogueBox dbox;
	protected CutsceneDbox dboxcs;
	protected int t = 0;
	[HideInInspector] public int ttt {
		get {
			return t;
		}
		set {
			t = Mathf.Min(dialogue.Length - 1, value);
		}
	}

	public DialogueArray[] dialogue = new DialogueArray[2];
	[HideInInspector] public Transform zoomTransform;
	private Vector3 defaultZoomOffset = new Vector3(-13.5f, 15.5f, 33);
	private Vector3 defaultZoomRotation = new Vector3(20, 135, 0);

	// Use this for initialization
	protected virtual void Start () {
		player = FindObjectOfType<Controllable>();
		Debug.Log("NPC player: " + player);
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
		zoomTransform = Array.Find(transform.GetComponentsInChildren<Transform>(), t => t.gameObject.name == "Zoom Transform");
		if(zoomTransform == null) zoomTransform = MakeZoomTransform();
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

	private Transform MakeZoomTransform() {
		GameObject zt = new GameObject("Zoom Transform");
		zt.transform.SetParent(transform);
		zt.transform.localPosition = defaultZoomOffset;
		zt.transform.localRotation = Quaternion.Euler(defaultZoomRotation);
		return zt.transform;
	}

	public virtual void Interact() {

	}
}
