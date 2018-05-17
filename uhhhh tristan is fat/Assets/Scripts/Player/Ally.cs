using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour {

	private int h = 100;
	public int hp {
		get {
			return h;
		}
		set {
			h = Mathf.Clamp(value, 0, 100);
			if(h == 0) {
				Die();
			}
		}
	}
	private int s = 100;
	public int st {
		get {
			return s;
		}
		set {
			s = Mathf.Clamp(value, 0, 100);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Die() {

	}
}
