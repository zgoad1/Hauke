using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHead : MonoBehaviour {

	// Should just be titled CharacterHead or something because it's used for non-NPCs but I'm too lazy to change all the references
	
	private Transform toFace;
	private Quaternion irot;
	private IEnumerator HeadTurnCR;
	private bool outOfRange = false;
	private Quaternion newRotation;
	private float maxXRot = 45;
	//public bool debug = false;
	[HideInInspector] public Transform body;

	// Use this for initialization
	void Start () {
		irot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		// Facing interactables (head turning)
		/*
		if(debug) {
			Debug.Log("body: " + body);
			Transform p = FindObjectOfType<Controllable>().transform;
			float angle = (body.transform.localEulerAngles.x + (Mathf.Atan2(transform.position.z - p.position.z, transform.position.x - p.position.x) / Mathf.PI * 180)) % 360;
			//Debug.Log("Side Y: " + (transform.position.z - p.position.z) + "\nSide X: " + (transform.position.x - p.position.x));
			//Debug.Log("Arctan: " + angle + "\nrotationX: " + body.transform.localEulerAngles.x);
		}
		*/
		if(toFace != null) {
			// limit rotation along x to 45, y to 30
			MTSBBI.LookAtXYZ(transform, toFace, 3, 0.1f);
			Vector3 newRot = transform.localEulerAngles;
			if(newRot.x >= 180) newRot.x = newRot.x - 360;
			if(newRot.y >= 180) newRot.y = newRot.y - 360;
			newRot.x = Mathf.Clamp(newRot.x, -maxXRot, maxXRot);
			newRot.y = Mathf.Clamp(newRot.y, -30, 30);
			newRot.z = 0;   // Unity likes to set z to ~350 for no distinguishable reason
			transform.localRotation = Quaternion.Euler(newRot);
		} else if(outOfRange) {
			transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, 0.1f);
		}
	}

	#region Head turning
	// SmoothTurn the head back to its original rotation
	public void LookBack() {
		SmoothTurnHead(irot);
		toFace = null;
		outOfRange = false;
	}

	// Slerp head to a new rotation Quaternion
	protected void SmoothTurnHead(Quaternion q) {
		HeadTurnCR = SmoothTurnHeadCR(transform, q, 0.2f);
		StartCoroutine(HeadTurnCR);
	}

	// Coroutine for above method
	protected IEnumerator SmoothTurnHeadCR(Transform t, Quaternion q, float lerpFac) {
		Debug.Log("Smoothturning head");
		for(int i = 0; i < 60; i++) {
			t.localRotation = Quaternion.Lerp(t.localRotation, q, lerpFac);
			yield return null;
		}
		Debug.Log("Finished head smoothturn");
	}

	// Set transform for the head to look at
	public void FaceTransform(Transform facingTransform) {
		if(HeadTurnCR != null) StopCoroutine(HeadTurnCR);
		toFace = facingTransform;
		if(toFace == null) LookBack();
		else if(body.GetComponent<Controllable>() == null) {	// there are bugs when the player tries to do this so I'm just doing this
			float angle = (body.transform.localEulerAngles.x + (Mathf.Atan2(transform.position.z - toFace.position.z, transform.position.x - toFace.position.x) / Mathf.PI * 180)) % 360;
			// if we're behind this head
			if(angle >= 90 && angle <= 270) {
				toFace = null;
				outOfRange = true;
				newRotation = Quaternion.Euler((int)Mathf.Sign(180 - angle) * (maxXRot + 10), 0, 0);
				Debug.Log("newRotation.x: " + newRotation.eulerAngles.x);
			}
		}
		if(facingTransform != null) Debug.Log("Setting face target of " + this + " to " + facingTransform.gameObject);
	}
	#endregion
}
