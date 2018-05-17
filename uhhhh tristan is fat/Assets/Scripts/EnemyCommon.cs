using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCommon : MonoBehaviour {

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
		agent.SetDestination(target.transform.position);
	}


}
