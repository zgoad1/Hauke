using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Animation>().Play();
	}
}
