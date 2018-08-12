using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBillboard : MonoBehaviour {

	[SerializeField] private Transform lookAtt;
	public static Transform lookAt;

	private void Reset() {
		lookAtt = FindObjectOfType<MainCamera>().transform;
		lookAt = lookAtt;
	}

	private void Start() {
		Reset();
	}

	// Update is called once per frame
	void Update () {
		transform.LookAt(GrassControl.newLookAt);
	}
}
