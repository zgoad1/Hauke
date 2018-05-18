using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCommon : Enemy {

	private NavMeshAgent agent;
	private Player player;
	//private Companion companion;
	private Ally target;

	// Use this for initialization
	void Start () {
		attacking = new bool[1];
		atkDamage = new int[] { 10 };	// hack

		maxHp = 100;

		agent = GetComponent<NavMeshAgent>();
		player = FindObjectOfType<Player>();
		//companion = FindObjectOfType<Companion>();
		target = player;								// TODO: Choose the player or the companion for target
	}
	
	void FixedUpdate () {
		if(agent.enabled) {
			agent.SetDestination(target.transform.position);
		}
	}

	public override void Knockback(Vector3 force) {
		if(!invincible) {
			GetComponent<Rigidbody>().AddForce(force);
			agent.enabled = false;
		}
	}
	
	private void OnCollisionEnter(Collision collision) {
		if(LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			agent.enabled = true;
		}
	}

	public override void Attack() {
		throw new System.NotImplementedException();
	}
}
