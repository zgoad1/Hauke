using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour {

	private static int count = 0;

	// Use this for initialization
	void Start () {
		/*
		if(count > 0) {
			Destroy(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
			count++;
		}
		*/

		// Keep it not Singleton so we can have different ones with different Dboxes and stuff

	}
}
