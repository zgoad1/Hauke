using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCommon : Enemy {

	protected NavMeshAgent agent;
	//protected Companion companion;
	protected Ally target;
	protected Vector3 knockbackOffset = new Vector3(0f, 0.5f, 0f);
	protected Vector2 prevPosition;
	protected Vector2 thisPosition;

	protected override void Reset() {
		base.Reset();
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	// Use this for initialization
	protected virtual void Start () {
		Reset();
		attacking = new bool[1];
		atkDamage = new int[] { 10 };	// hack

		maxHp = 100;

		//companion = FindObjectOfType<BattleCompanion>();
		target = player;								// TODO: Choose the player or the companion for target
	}

	protected virtual void Update() {
		thisPosition.x = transform.position.x;
		thisPosition.y = transform.position.z;
		float speed = Vector2.Distance(prevPosition, thisPosition);
		prevPosition.x = transform.position.x;
		prevPosition.y = transform.position.z;
		if(anim != null) {
			anim.SetFloat("speed", speed);
		}
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
	
	protected void OnCollisionEnter(Collision collision) {
		if(LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			agent.enabled = true;
		}
	}

	public override void Attack() {
		Debug.Log("Enemy attacking");
	}
}
