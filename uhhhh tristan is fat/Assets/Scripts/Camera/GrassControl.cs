using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassControl : MonoBehaviour {
	
	public static Vector3 newLookAt;

	// Use this for initialization
	void Start () {
		newLookAt = GrassBillboard.lookAt.position;
	}
	
	// Update is called once per frame
	void Update () {
		newLookAt = GrassBillboard.lookAt.position;
		newLookAt.y = transform.position.y;
	}
}
