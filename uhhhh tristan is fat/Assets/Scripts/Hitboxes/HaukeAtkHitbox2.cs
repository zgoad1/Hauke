using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hauke's spin attack hitbox
public class HaukeAtkHitbox2 : AHBType2 {

	protected override void Reset() {
		base.Reset();
		me = FindObjectOfType<BattlePlayer>();
	}

	protected void Awake() {
		Reset();
		hbIndex = 1;
		isAlly = true;
	}

	protected override void Hit(Rigidbody hit) {
		Enemy enemy = hit.gameObject.GetComponent<Enemy>();
		enemy.Knockback((hit.transform.position - me.transform.position).normalized * 2000);
		base.Hit(hit);
	}
}
