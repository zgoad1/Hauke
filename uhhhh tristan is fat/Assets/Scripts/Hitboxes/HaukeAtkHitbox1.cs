using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaukeAtkHitbox1 : AHBType2 {

	/** Hitbox for Hauke's boomerang. Attacking will trigger an animation; 
	 * "attacking" stays true until it's ended. If we hit "me" at any point
	 * during the animation, stop. If the animation has ended and we're not
	 * touching "me," increment the velocity towards the player until collision.
	 * If at any point we hit the ground, stop.
	 */

	/* FOR FUTURE POLISH:
	 * Tilt increases slightly with lower velocity (lookAt player and tilt on x-axis)
	 */

	[SerializeField] private Transform parentT;

	private enum state {
		animating = 1,
		returning = 2
	}
	private state s = state.animating;
	private Rigidbody rb;
	private Animator anim;
	private Vector3 prevPos = Vector3.zero;
	[SerializeField] private Vector3 playerOffset = new Vector3(0f, 3f, 0f);	// how high up to throw from
	private float startOffset = 3f;	// how far forward from the player we should start
	private float iFollowFactor = 0f;
	private float followFactor = 0f;
	private Transform camT;
	//private int rotOffset = -90;

	// Use this for initialization
	void Start () {
		Reset();
		isAlly = true;
		hbIndex = 1;
		prevPos = transform.position;
	}

	protected override void Reset() {
		base.Reset();
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		camT = FindObjectOfType<MainCamera>().transform;
	}

	/* Resets the transform, stops
	 * the animation, and deactivates the object.
	 */
	protected void ResetBoomerang() {
		Debug.Log("Resetting boomerang");
		anim.enabled = true;
		rb.isKinematic = true;
		s = state.animating;
		MTSBBI.SetActiveChildren(parentT, false);
		me.attacking[hbIndex] = false;
		followFactor = iFollowFactor;
		((BattlePlayer)me).CatchWeapon();
	}

	// Update is called once per frame
	void Update () {
		switch(s) {
			case state.animating:
				rb.velocity = (transform.position - prevPos) * 60;
				//Debug.Log("velocity: " + rb.velocity);
				break;
			case state.returning:
				Debug.Log("attepmtnthing to return boomeroh");
				Vector3 distance = (me.transform.position + playerOffset) - transform.position;
				Vector3 toPlayer;
				if(Mathf.Abs(distance.magnitude) < 5) {
					// keep the vector from going below 5 in magnitude by multiplying it by a scalar
					// (if it slows down too much, it'll just follow the player as they move)
					float alpha = 25 / distance.magnitude;
					toPlayer = alpha * distance * 5;
				} else {
					toPlayer = distance * 5;
				}
				rb.velocity = (1 - followFactor) * rb.velocity + followFactor * toPlayer;
				if(followFactor < 1) followFactor += 0.001f;
				break;
		}
		prevPos = transform.position;
	}

	protected void OnTriggerEnter(Collider other) {
		// reset if we collide with Hauke or we hit the ground
		if(other.GetComponent<BattlePlayer>() != null || LayerMask.LayerToName(other.gameObject.layer) == "Ground") {
			ResetBoomerang();
		}
	}

	protected override void Hit(Rigidbody hit) {
		Enemy enemy = hit.gameObject.GetComponent<Enemy>();
		enemy.Knockback(rb.velocity.normalized * 2000);
		base.Hit(hit);
	}

	public void Begin() {
		Reset();
		Vector3 newForward = new Vector3(camT.forward.x, 0f, camT.forward.z).normalized;
		//me.transform.forward = newForward;
		parentT.position = me.transform.position + playerOffset + startOffset * newForward;
		parentT.forward = camT.forward;
		if(((BattlePlayer)me).onGround || ((BattlePlayer)me).canStillJump) {
			Vector3 newEuler = parentT.rotation.eulerAngles;
			newEuler.x = newEuler.x < 180 ? 0 : newEuler.x;
			parentT.rotation = Quaternion.Euler(newEuler);
		}
	}

	private void FinishAnim() {
		Debug.Log("Finished boomerang animation");
		anim.enabled = false;
		rb.isKinematic = false;
		s = state.returning;
	}
}
