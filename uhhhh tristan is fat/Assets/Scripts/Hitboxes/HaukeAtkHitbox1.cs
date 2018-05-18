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
	 * Height of boomerang increases exponentially with distance from player.
	 * Tilt increases slightly with lower velocity (lookAt player and tilt)
	 */

	[SerializeField] private Transform parentT;

	private enum state {
		animating = 1,
		returning = 2
	}
	private state s = state.animating;
	//private Vector3 iPos;
	private Rigidbody rb;
	private Animator anim;
	private Vector3 prevPos = Vector3.zero;
	private Vector3 playerOffset = new Vector3(0f, 3f, 0f);
	private float startOffset = 3f;
	private float iFollowFactor = 0f;
	private float followFactor = 0f;

	// Use this for initialization
	void Start () {
		isAlly = true;
		me = FindObjectOfType<Player>();
		hbIndex = 1;
		//iPos = transform.position;
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		prevPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		switch(s) {
			case state.animating:
				rb.velocity = (transform.position - prevPos) * 60;
				Debug.Log(rb.velocity);
				break;
			case state.returning:
				Vector3 distance = (me.transform.position + playerOffset) - transform.position;
				Vector3 asdf;
				if(Mathf.Abs(distance.magnitude) < 5) {
					// do some maths
					float alpha = 25 / distance.magnitude;
					asdf = alpha * distance * 5;
				} else {
					asdf = distance * 5;
				}
				rb.velocity = (1 - followFactor) * rb.velocity + followFactor * asdf;
				if(followFactor < 1) followFactor += 0.001f;
				Debug.Log(rb.velocity);
				break;
		}
		prevPos = transform.position;
	}

	protected void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Player>() != null || LayerMask.LayerToName(other.gameObject.layer) == "Ground") {
			Reset();
		}
	}

	protected override void Hit(Rigidbody rb) {
		Enemy enemy = rb.gameObject.GetComponent<Enemy>();
		enemy.Knockback(transform.forward * 2000 + verticalForce);
		enemy.TakeDamage(me.atkDamage[hbIndex]);
	}

	public void Begin() {
		me = FindObjectOfType<Player>();	// this is necessary because it can't find it in Start() (?????????????????)
		//Debug.Log("parentT: " + parentT + "\nme: " + me);
		parentT.SetPositionAndRotation(me.transform.position + playerOffset + startOffset * me.transform.forward, me.transform.rotation);
	}

	private void FinishAnim() {
		Debug.Log("Finished animation");
		anim.enabled = false;
		rb.isKinematic = false;
		s = state.returning;
	}

	/* Parents the object back to the player, resets the transform, stops
	 * the animation, and deactivates the object.
	 */
	private void Reset() {
		Debug.Log("Resetting boomerang");
		anim.enabled = true;
		rb.isKinematic = true;
		s = state.animating;
		MTSBBI.SetActiveChildren(parentT, false);
		me.attacking[hbIndex] = false;
		followFactor = iFollowFactor;
	}
}
