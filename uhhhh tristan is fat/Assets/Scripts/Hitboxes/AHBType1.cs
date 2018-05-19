using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHBType1 : AtkHitbox {

	/** Attack hitboxes that damage all enemies in the range once,
	 * then deactivate until the next attack.
	 * 
	 * SET UPON START:
	 * - isAlly
	 * - me
	 * - hbIndex
	 */

	protected List<Rigidbody> collision = new List<Rigidbody>();

	private void FixedUpdate() {
		if(isAttacking()) {
			Hit(null);
			me.attacking[hbIndex] = false;
		}
	}

	// A list of Rigidbodies is kept so we can damage them all at once.
	// We look for RBs from the BattleEntity subtype opposite of ours.

	// Add objects entering the collision box to the list
	private void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Rigidbody>() != null && isAlly? other.GetComponent<Enemy>() != null : other.GetComponent<Ally>() != null) {
			collision.Add(other.GetComponent<Rigidbody>());
		}
	}

	// Remove objects leaving the collision box from the list
	private void OnTriggerExit(Collider other) {
		if(other.GetComponent<Rigidbody>() != null && isAlly ? other.GetComponent<Enemy>() != null : other.GetComponent<Ally>() != null) {
			collision.Remove(other.GetComponent<Rigidbody>());
		}
	}

	protected override void Hit(Rigidbody hit) {
		List<Rigidbody> toRemove = new List<Rigidbody>();
		foreach(Rigidbody rb in collision) {
			Enemy enemy = rb.gameObject.GetComponent<Enemy>();
			enemy.TakeDamage(me.atkDamage[hbIndex]);
			if(enemy.hp == 0) toRemove.Add(rb);
		}
		foreach(Rigidbody rb in toRemove) collision.Remove(rb);
		base.Hit(hit);
	}
}
