using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTrucker : EnemyCommon {
	protected override void Start() {
		base.Start();
		anim.speed = 0.3f;
	}

	protected override void Update() {
		base.Update();
		if(Vector3.Distance(transform.position, player.transform.position) < agent.stoppingDistance + 2) {
			Attack();
			Debug.LogWarning("ATTACKING PLAYER");
		} else {
			Debug.Log("Distance to player: " + Vector3.Distance(transform.position, player.transform.position));
		}
	}

	public override void Attack() {
		anim.SetTrigger("attack");
		attacking[0] = true;
	}
}
