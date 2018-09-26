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
	[HideInInspector] public bool attacking = false;
	[HideInInspector] public Vector3 iScale;

	protected virtual void Start() {
		iScale = transform.localScale;
	}

	private void FixedUpdate() {
		if(IsAttacking()) {
			Hit(null);
		}
	}

	protected override bool IsAttacking() {
		return attacking;
	}

	// A list of Rigidbodies is kept so we can damage them all at once.
	// We look for RBs from the BattleEntity subtype opposite of ours.

	// Add objects entering the collision box to the list
	private void OnTriggerEnter(Collider other) {
		//Debug.Log("isAlly: " + isAlly + "\nenemy: " + other.GetComponent<Enemy>() + "\nAlly: " + other.GetComponent<Ally>());
		if(other.GetComponent<Rigidbody>() != null && (isAlly ? other.GetComponent<Enemy>() != null : other.GetComponent<Ally>() != null)) {
			//Debug.Log("Adding " + other.gameObject.name + " to hitbox list: " + gameObject.name);
			collision.Add(other.GetComponent<Rigidbody>());
		}
	}

	// Remove objects leaving the collision box from the list
	private void OnTriggerExit(Collider other) {
		if(other.GetComponent<Rigidbody>() != null && (isAlly ? other.GetComponent<Enemy>() != null : other.GetComponent<Ally>() != null)) {
			//Debug.Log("Removing " + other.gameObject.name + " from hitbox list: " + gameObject.name);
			collision.Remove(other.GetComponent<Rigidbody>());
		}
	}

	protected override void Hit(Rigidbody hit) {
		base.Hit(hit);
		List<Rigidbody> toRemove = new List<Rigidbody>();
		foreach(Rigidbody rb in collision) {
			//Debug.Log(gameObject.name + ": damaging enemy");
			BattleEntity entity = rb.gameObject.GetComponent<BattleEntity>();
			entity.TakeDamage(me.atkDamage[hbIndex]);
			if(entity.hp == 0) toRemove.Add(rb);
		}
		foreach(Rigidbody rb in toRemove) collision.Remove(rb);
		attacking = false;
		//me.attacking[hbIndex] = false;
		transform.localScale = iScale;
	}
}
