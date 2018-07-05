using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Ally : BattleEntity {

	/** BattlePlayer and companion.
	 * 
	 * SET UPON START: 
	 * - maxHp
	 * - attacking (array)
	 * - atkDamage (array & elements)
	 */
	[HideInInspector] public Transform head;
	private float s = 100;
	protected float maxSt = 100;
	protected float st {
		get {
			return s;
		}
		set {
			s = Mathf.Clamp(value, 0, maxSt);
		}
	}

	protected virtual void Start() {
		head = Array.Find(GetComponentsInChildren<Transform>(), t => t.gameObject.name == "Head");
	}
}
