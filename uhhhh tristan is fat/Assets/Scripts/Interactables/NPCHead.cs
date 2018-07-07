using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHead : MonoBehaviour {

	// Should just be titled CharacterHead or something because it's used for non-NPCs but I'm too lazy to change all the references
	
	private Transform toFace;
	private Quaternion irot;
	private IEnumerator HeadTurnCR;

	// Use this for initialization
	void Start () {
		irot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		// Facing interactables (head turning)
		if(toFace != null) {
			// limit rotation along x to 70, y to 30
			MTSBBI.LookAtXYZ(transform, toFace, 3, 0.1f);
			Vector3 newRot = transform.localEulerAngles;
			if(newRot.x >= 180) newRot.x = newRot.x - 360;
			if(newRot.y >= 180) newRot.y = newRot.y - 360;
			newRot.x = Mathf.Clamp(newRot.x, -70, 70);
			newRot.y = Mathf.Clamp(newRot.y, -30, 30);
			newRot.z = 0;	// Unity likes to set z to ~350 for no distinguishable reason
			transform.localRotation = Quaternion.Euler(newRot);
		}
	}

	#region Head turning
	// SmoothTurn the head back to its original rotation
	public void LookBack() {
		SmoothTurnHead(irot);
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
		if(facingTransform != null) Debug.Log("Setting face target of " + this + " to " + facingTransform.gameObject);
	}
	#endregion
}
