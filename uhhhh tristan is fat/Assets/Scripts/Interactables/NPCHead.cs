using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHead : MonoBehaviour {
	
	private Transform toFace;
	private Quaternion irot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(toFace != null) {
			transform.LookAt(toFace);
		}
	}

	public void SetFacing(Transform t) {
		toFace = t;
		irot = transform.localRotation;
	}
}
