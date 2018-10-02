using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour {

	public HaukeAtkHitbox1 hb;

	private void Reset() {
		hb = GetComponentInChildren<HaukeAtkHitbox1>();
	}

	// Start doesn't work here
	void Awake () {
		hb = GetComponentInChildren<HaukeAtkHitbox1>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
