using System.Collections;
using UnityEngine;

public class Player : Ally {

	[SerializeField] private Transform camTransform;
	[SerializeField] private float speed = 0.3f;
	[SerializeField] private float accel = 0.3f;
	[SerializeField] private float decel = 0.3f;
	[SerializeField] private float jumpForce = 0.5f;
	[SerializeField] private float grav = 0.03f;

	private bool og = false;	// whether Percy can jump
	public bool onGround {
		get {
			return og;
		}
		set {
			og = value;
			if(value) {
				StopCoroutine("CanStillJump");
				StartCoroutine("CanStillJump");
			}
		}
	}
	public Vector3 movDirec = Vector3.zero;    // direction of movement

	private CharacterController cc;
	private Vector3 ipos;   // keeps track of starting position so we can return
	private float rightKey;
	private float fwdKey;
	private bool jKey;
	private bool dodgeKey;
	private float rightMov = 0f;
	private float fwdMov = 0f;
	private float upMov = 0f;
	private Quaternion playerRot = new Quaternion(0f, 0f, 0f, 0f);
	private Vector3 hitNormal = Vector3.zero;
	private bool notOnSlope = false;
	private bool canStillJump = true;
	private bool dodging = false;
	private float stopSpeed = 0.075f;

	public void Die() {
		Debug.Log("ded");
		gameObject.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Use this for initialization
	void Start() {
		ipos = transform.position;
		cc = GetComponent<CharacterController>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update() {
		//controls
		rightKey = Input.GetAxisRaw("Horizontal");
		fwdKey = Input.GetAxisRaw("Vertical");
		jKey = Input.GetButtonDown("Jump")? true : jKey;
		dodgeKey = Input.GetButtonDown("Run")? true : dodgeKey;	

		// change forward's y to 0 then normalize, in case the camera is pointed down or up
		Vector3 tempForward = camTransform.forward;
		tempForward.y = 0f;

		if(!dodging) {

			if(rightKey != 0) {
				rightMov = Mathf.Lerp(rightMov, (rightKey * speed), accel);
			} else {
				rightMov = Mathf.Lerp(rightMov, 0f, decel);
			}
			if(fwdKey != 0) {
				fwdMov = Mathf.Lerp(fwdMov, (fwdKey * speed), accel);
			} else {
				fwdMov = Mathf.Lerp(fwdMov, 0f, decel);
			}

			// pausing
			if(Input.GetButtonDown("Pause")) {
				if(Cursor.lockState != CursorLockMode.Locked) {
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				} else {
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}

			// get movement direction
			movDirec = tempForward.normalized * fwdMov + camTransform.right.normalized * rightMov;

			// return to starting point
			if(Input.GetKeyDown(KeyCode.Return)) {
				transform.position = ipos + new Vector3(0, 5, 0);
			}

			if(dodgeKey && (onGround || canStillJump)) {
				StartCoroutine("Dodge");
				canStillJump = false;
			}
		}
		//if(jKey) Debug.LogWarning("Jumping\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
		//if(dodgeKey) Debug.LogWarning("Dodging\ndodgeKey = " + dodgeKey + "\nonGround = " + onGround);

		// physics calculations (executions go in FixedUpdate)
		onGround = false;
	}

	// called once per physics update
	void FixedUpdate() {
		// calculate movement
		movDirec.y = upMov;
		//Character sliding of surfaces
		float slideFriction = 0.5f;
		if(!notOnSlope) {
			movDirec.x += -upMov * hitNormal.x * (1f - slideFriction);
			movDirec.z += -upMov * hitNormal.z * (1f - slideFriction);
			hitNormal = Vector3.zero;
			onGround = false;
			canStillJump = false;
		}
		cc.Move(movDirec);  // triggers collision detection
		if(!notOnSlope) {
			Debug.Log("hit angle: " + Vector3.Angle(Vector3.up, hitNormal));
		}
		transform.forward = Vector3.Lerp(transform.forward, movDirec, 0.6f);
		playerRot.y = transform.rotation.y;
		playerRot.w = transform.rotation.w;
		transform.rotation = playerRot;

		// jumping
		if(jKey && (onGround || canStillJump) && !dodging) {
			onGround = false;
			Debug.LogWarning("Jumping\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
			upMov = jumpForce;
			canStillJump = false;
		} else if(!onGround || !notOnSlope) {
			upMov -= grav;
			//Debug.Log("Increasing gravity: " + upMov);
			if(jKey) Debug.Log("Falling\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
		} else {
			upMov = -grav;
			if(jKey) Debug.LogWarning("Apex\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
		}
		jKey = false;		// keep these true after they're pressed until FixedUpdate is called
		dodgeKey = false;

		if(!dodging) {
			movDirec.x = 0f;
			movDirec.z = 0f;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.gameObject.tag != "Transparent") {
			hitNormal = hit.normal;
			// notOnSlope = we're on ground level enough to walk OR we're hitting a straight-up wall
			notOnSlope = Vector3.Angle(Vector3.up, hitNormal) <= cc.slopeLimit || Vector3.Angle(Vector3.up, hitNormal) >= 89;
			if(movDirec.y <= 0 && hit.point.y < transform.position.y + .5f) {
				// hit ground
				if(notOnSlope) onGround = true;
				Debug.Log("Hit ground");
				// else if the hit point is from above and inside our radius (on top of head rather than on outer edge)
			} else if(hit.point.y > transform.position.y + .5f && Mathf.Sqrt(Mathf.Pow(transform.position.x - hit.point.x, 2) + Mathf.Pow(transform.position.z - hit.point.z, 2)) < cc.radius) {
				// hit something going up
				upMov = Mathf.Min(0f, upMov);
				Debug.Log("I hit my head!");
			} else {
				Debug.Log("Actually hit nothing!");
			}
		}
	}

	// Grace period in which you can still jump after moving off of an edge
	private IEnumerator CanStillJump() {
		canStillJump = true;
		yield return new WaitForSeconds(0.2f);
		canStillJump = false;
	}

	private IEnumerator Dodge() {
		dodging = true;
		movDirec = (Mathf.Abs(movDirec.x) - stopSpeed <= 0 && Mathf.Abs(movDirec.z) - stopSpeed <= 0)? camTransform.forward * speed * 3f : movDirec.normalized * speed * 3f;
		transform.forward = movDirec;
		yield return new WaitForSeconds(0.3f);
		dodging = false;
	}
}
