using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCommon : Enemy {

	private NavMeshAgent agent;
	private BattlePlayer player;
	//private Companion companion;
	private Ally target;
	private Vector3 knockbackOffset = new Vector3(0f, 0.5f, 0f);

	// Use this for initialization
	void Start () {
		attacking = new bool[1];
		atkDamage = new int[] { 10 };	// hack

		maxHp = 100;

		agent = GetComponent<NavMeshAgent>();
		player = FindObjectOfType<BattlePlayer>();
		//companion = FindObjectOfType<BattleCompanion>();
		target = player;								// TODO: Choose the player or the companion for target
	}
	
	void FixedUpdate () {
		if(agent.enabled) {
			agent.SetDestination(target.transform.position);
		}
	}

	public override void Knockback(Vector3 force) {
		if(!invincible) {
			if(agent.enabled) transform.position += knockbackOffset;	// only force them up if they're on the ground, because that's the only place we have that problem
			GetComponent<Rigidbody>().AddForce(force + verticalForce);
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
