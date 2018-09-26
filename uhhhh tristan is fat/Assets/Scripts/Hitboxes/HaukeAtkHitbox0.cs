using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaukeAtkHitbox0 : AHBType1 {

	// hitbox for Hauke's standard hack

	protected Transform camT;
	protected Transform meT;
	protected int rotOffset = -90;	// to keep the rotation from aiming towards the ground

	// Use this for initialization
	protected override void Start () {
		base.Start();
		Reset();
		if(!(this is HaukeAtkHitbox3)) {    // darn my bad coding
			isAlly = true;
			hbIndex = 0;
		}
	}

	protected override void Reset() {
		base.Reset();
		me = FindObjectOfType<BattlePlayer>();
		meT = me.transform;
		camT = FindObjectOfType<MainCamera>().transform;
	}

	// Update is called once per frame
	void Update () {
		float newx = camT.rotation.eulerAngles.x + rotOffset;
		transform.rotation = Quaternion.Euler(newx < 0? rotOffset: newx, meT.rotation.eulerAngles.y, meT.rotation.eulerAngles.z);
	}

	protected override void Hit(Rigidbody hit) {
		if(!(this is HaukeAtkHitbox3)) {	// HAHB3 has its own knockback formula so it needs to skip this part
			Debug.Log(gameObject.name + ": exerting hitbox");
			// apply knockback to all enemies in range...
			foreach(Rigidbody rb in collision) {
				Enemy enemy = rb.gameObject.GetComponent<Enemy>();
				enemy.Knockback(meT.forward * 2000);
			}
		}
		// ...in addition to the standard procedures
		base.Hit(hit);
	}
}
