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
		agent = GetComponent<NavMeshAgent>();
		player = FindObjectOfType<Player>();
		//companion = FindObjectOfType<Companion>();
		target = player;
	}
	
	void FixedUpdate () {
		if(agent.enabled) {
			agent.SetDestination(target.transform.position);
		}
	}

	public override void Knockback() {
		agent.enabled = false;
	}
	
	private void OnCollisionEnter(Collision collision) {
		if(LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			agent.enabled = true;
		}
	}
}
