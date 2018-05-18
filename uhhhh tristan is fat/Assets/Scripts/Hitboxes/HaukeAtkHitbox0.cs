using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaukeAtkHitbox0 : AHBType1 {

	// hitbox for Hauke's standard hack

	private Transform camT;
	private Transform meT;
	private int rotOffset = -90;

	// Use this for initialization
	void Start () {
		isAlly = true;
		me = FindObjectOfType<Player>();
		hbIndex = 0;
		camT = FindObjectOfType<MainCamera>().transform;
		meT = me.transform;
	}
	
	// Update is called once per frame
	void Update () {
		float newx = camT.rotation.eulerAngles.x + rotOffset;
		transform.rotation = Quaternion.Euler(newx < 0? rotOffset: newx, meT.rotation.eulerAngles.y, meT.rotation.eulerAngles.z);
	}

	protected override void Hit(Rigidbody hit) {
		foreach(Rigidbody rb in collision) {
			Enemy enemy = rb.gameObject.GetComponent<Enemy>();
			enemy.Knockback(meT.forward * 2000 + verticalForce);
			enemy.TakeDamage(me.atkDamage[hbIndex]);
		}
	}
}
