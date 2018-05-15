using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField] private Transform camTransform;
	[SerializeField] private float speed = 0.3f;
	[SerializeField] private float accel = 0.3f;
	[SerializeField] private float decel = 0.3f;
	[SerializeField] private float jumpForce = 3f;
	[SerializeField] private float grav = 0.2f;

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
	private float rightMov = 0f;
	private float fwdMov = 0f;
	private float upMov = 0f;
	private Quaternion playerRot = new Quaternion(0f, 0f, 0f, 0f);
	private Vector3 hitNormal = Vector3.zero;
	private bool notOnSlope = false;
	private bool dodgeKey = false;
	private bool canStillJump = true;
	private bool dodging = false;

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
		jKey = Input.GetButtonDown("Jump");    // sometimes this is inaccurate - doesn't update when it should. Updates when another button is pressed though, so it works in motion. Close enough
		dodgeKey = Input.GetButtonDown("Run");	// use this as more of a dodge

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

			if(dodgeKey && onGround) {
				StartCoroutine("Dodge");
			}

			onGround = false;
		}
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
		}
		cc.Move(movDirec);  // triggers collision detection
		// notOnSlope = we're on ground level enough to walk OR we're hitting a straight-up wall
		notOnSlope = Vector3.Angle(Vector3.up, hitNormal) <= cc.slopeLimit || Vector3.Angle(Vector3.up, hitNormal) >= 89;
		if(!notOnSlope) {
			Debug.Log("hit angle: " + Vector3.Angle(Vector3.up, hitNormal));
		}
		transform.forward = Vector3.Lerp(transform.forward, movDirec, 0.6f);
		playerRot.y = transform.rotation.y;
		playerRot.w = transform.rotation.w;
		transform.rotation = playerRot;
		if(!dodging) {
			// jumping
			if(jKey && (onGround || canStillJump)) {
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
			
			movDirec.x = 0f;
			movDirec.z = 0f;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.gameObject.tag != "Transparent") {
			hitNormal = hit.normal;
			if(movDirec.y <= 0 && hit.point.y < transform.position.y - .5f) {
				// hit ground
				onGround = true;
				//Debug.Log("Hit ground");
			// else if the hit point is from above and inside our radius (on top of head rather than on outer edge)
			} else if(hit.point.y > transform.position.y + .5f && Mathf.Sqrt(Mathf.Pow(transform.position.x - hit.point.x, 2) + Mathf.Pow(transform.position.z - hit.point.z, 2)) < cc.radius) {
				// hit something going up
				upMov = Mathf.Min(0f, upMov);
				Debug.Log("I hit my head!");
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
		movDirec = (movDirec == Vector3.zero)? transform.forward * speed * 2f : movDirec.normalized * speed * 2f;
		yield return new WaitForSeconds(0.4f);
		dodging = false;
	}
}
