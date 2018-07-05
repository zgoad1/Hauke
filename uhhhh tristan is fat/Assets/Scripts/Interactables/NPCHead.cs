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
			MTSBBI.LookAtXYZ(transform, toFace, 3, 0.2f);
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
		for(int i = 0; i < 60; i++) {
			t.localRotation = Quaternion.Slerp(t.localRotation, q, lerpFac);
			yield return null;
		}
	}

	// Set transform for the head to look at
	public void FaceTransform(Transform facingTransform) {
		if(HeadTurnCR != null) StopCoroutine(HeadTurnCR);
		toFace = facingTransform;
		if(toFace == null) LookBack();
	}
	#endregion
}
