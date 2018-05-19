using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkHitbox : MonoBehaviour {

	/** Any attack hitbox.
	 * 
	 * SET UPON START:
	 * - isAlly
	 * - me
	 * - hbIndex
	 */

	protected BattleEntity me;
	protected int hbIndex;
	protected bool isAlly;

	protected bool isAttacking() {
		return me.attacking[hbIndex];
	}

	protected virtual void Hit(Rigidbody hit) {
		Debug.Log("Damaging enemy for " + me.atkDamage[hbIndex]);
	}
}
