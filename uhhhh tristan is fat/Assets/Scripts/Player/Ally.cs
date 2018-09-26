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
	[HideInInspector] public NPCHead head;
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

	protected override void Start() {
		base.Start();
		Transform h = Array.Find(GetComponentsInChildren<Transform>(), t => t.gameObject.name == "Head");
		head = h.GetComponent<NPCHead>();
		if(head == null) {
			head = h.gameObject.AddComponent<NPCHead>();
		}
	}
}
