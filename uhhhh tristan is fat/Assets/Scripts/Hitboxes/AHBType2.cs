using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHBType2 : AtkHitbox {

	/** Attack hitboxes that exert TakeDamage() on everything
	 * they touch for every frame they're active. Use SetActive()
	 * to begin (or a Begin method)
	 * 
	 * SET UPON START:
	 * - isAlly
	 * - me
	 * - hbIndex
	 */

	protected void OnTriggerStay(Collider other) {
		Rigidbody rb = other.GetComponent<Rigidbody>();
		if(isAttacking() && rb != null && isAlly ? other.GetComponent<Enemy>() != null : other.GetComponent<Ally>() != null) {
			Hit(rb);
		}
	}

	protected override void Hit(Rigidbody hit) {
		Enemy enemy = hit.gameObject.GetComponent<Enemy>();
		enemy.TakeDamage(me.atkDamage[hbIndex]);
		base.Hit(hit);
	}
}
