using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaukeAtkHitbox3 : HaukeAtkHitbox0 {

	protected override void Start() {
		base.Start();
		isAlly = true;
		hbIndex = 2;
	}

	protected override void Hit(Rigidbody hit) {
		Debug.Log(gameObject.name + ": exerting hitbox");
		// apply knockback to all enemies in range...
		foreach(Rigidbody rb in collision) {
			//Debug.Log("Damaging enemy: " + rb.gameObject.name);
			Enemy enemy = rb.gameObject.GetComponent<Enemy>();
			enemy.Knockback((rb.transform.position - meT.position).normalized * 4000);
		}
		// ...in addition to the standard procedures
		base.Hit(hit);
	}
}
