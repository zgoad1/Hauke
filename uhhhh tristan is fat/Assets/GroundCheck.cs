using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

	private Player p;

	// Use this for initialization
	void Start () {
		p = FindObjectOfType<Player>();		
	}

	// Update is called once per frame
	private void OnTriggerStay(Collider other) {
		//p.onGround = true;
		//Debug.Log("On ground");
	}

	private void OnTriggerExit(Collider other) {
		//p.onGround = false;
		//Debug.Log("Left ground");
	}
}
