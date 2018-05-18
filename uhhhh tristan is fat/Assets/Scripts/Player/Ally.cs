using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : BattleEntity {

	/** Player and companion.
	 * 
	 * SET UPON START: 
	 * - maxHp
	 * - attacking (array)
	 * - atkDamage (array & elements)
	 */

	private int s = 100;
	protected int st {
		get {
			return s;
		}
		set {
			s = Mathf.Clamp(value, 0, 100);
		}
	}

}
