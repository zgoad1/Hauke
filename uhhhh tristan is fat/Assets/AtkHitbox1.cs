using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkHitbox1 : MonoBehaviour {

	private Transform cam;
	private Transform player;
	private Quaternion offset = new Quaternion(-90, 0, 0, 0);
	private bool attack1 = false;
	private List<Rigidbody> collision = new List<Rigidbody>();
	private Vector3 forceOffset = new Vector3(0, 2000, 0);

	// Use this for initialization
	void Start () {
		cam = FindObjectOfType<MainCamera>().transform;
		player = FindObjectOfType<Player>().transform;
	}
	
	// Update is called once per frame
	void Update () {
		attack1 = Input.GetButtonDown("Fire1")? true : attack1;
		float newx = cam.rotation.eulerAngles.x - 90;
		transform.rotation = Quaternion.Euler(newx < 0? -90 : newx, player.rotation.eulerAngles.y, player.rotation.eulerAngles.z);
	}

	private void FixedUpdate() {
		if(attack1) {
			foreach(Rigidbody rb in collision) {
				rb.gameObject.GetComponent<Enemy>().Knockback();
				Debug.Log("Exerting force upon " + rb.gameObject + " ||| force: " + (player.forward * 2000 + forceOffset));
				rb.AddForce(player.forward * 2000 + forceOffset);
			}
			attack1 = false;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Rigidbody>() != null && other.tag == "Enemy") {
			collision.Add(other.GetComponent<Rigidbody>());
		}
		/*
		Debug.Log("Considering exerting force upon " + other.gameObject);
		if(attack1) {
			Rigidbody rb = other.GetComponent<Rigidbody>();
			Debug.Log("Really considering exerting force upon " + other.gameObject);
			if(rb != null && other.tag == "Enemy") {
				Debug.Log("Exerting force upon " + other.gameObject);
				rb.AddForce(player.forward * 1600);
			}
		}
		*/
	}

	private void OnTriggerExit(Collider other) {
		if(other.GetComponent<Rigidbody>() != null && other.tag == "Enemy") {
			collision.Remove(other.GetComponent<Rigidbody>());
		}
	}

	/*
private void OnCollisionStay(Collision coll) {
	Collider other = coll.collider;
	Debug.Log("Considering exerting force upon " + other.gameObject);
	if(attack1) {
		Rigidbody rb = other.GetComponent<Rigidbody>();
		Debug.Log("Really considering exerting force upon " + other.gameObject);
		if(rb != null && other.tag == "Enemy") {
			Debug.Log("Exerting force upon " + other.gameObject);
			rb.AddForce(player.forward * 1600);
		}
	}
}
*/
}
