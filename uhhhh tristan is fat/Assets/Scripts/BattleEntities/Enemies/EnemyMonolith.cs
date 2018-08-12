using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonolith : Enemy {
	
	private static Vector3[] points;	// places for Monoliths to spawn/teleport

	// Use this for initialization
	void Start () {
		Reset();

		maxHp = 1000;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(player.transform);
	}
}
