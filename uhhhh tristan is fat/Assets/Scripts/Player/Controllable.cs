using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class Controllable : Ally {

	[SerializeField] protected Transform camTransform;
	[SerializeField] protected float speed = 0.225f;
	[SerializeField] protected float accel = 0.175f;
	[SerializeField] protected float decel = 0.2f;
	[SerializeField] protected float grav = 0.03f;

	private bool og = false;    // whether Percy can jump
	[HideInInspector]
	public bool onGround {
		get {
			return og;
		}
		set {
			og = value;
		}
	}
	[HideInInspector] public Vector3 movDirec = Vector3.zero;    // direction of movement

	protected CharacterController cc;
	protected Vector3 ipos;   // keeps track of starting position so we can return
	protected float rightKey;
	protected float fwdKey;
	protected float rightMov = 0f;
	protected float fwdMov = 0f;
	protected float upMov = 0f;
	protected Quaternion playerRot = new Quaternion(0f, 0f, 0f, 0f);
	protected Vector3 hitNormal = Vector3.zero;
	protected bool notOnSlope = false;
	protected float stopSpeed = 0.075f;
	protected CameraControl cam;
	protected float camDist;

	// Use this for initialization
	protected virtual void Start() {
		ipos = transform.position;
		cc = GetComponent<CharacterController>();
		cam = FindObjectOfType<CameraControl>();
		camDist = cam.idistance;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Update is called once per frame
	protected virtual void Update() {
		//controls
		rightKey = Input.GetAxisRaw("Horizontal");
		fwdKey = Input.GetAxisRaw("Vertical");

		// change forward's y to 0 then normalize, in case the camera is pointed down or up
		Vector3 tempForward = camTransform.forward;
		tempForward.y = 0f;

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

		// physics calculations (executions go in FixedUpdate)
		onGround = false;

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
		transform.forward = Vector3.Lerp(transform.forward, movDirec, 0.6f);
		playerRot.y = transform.rotation.y;
		playerRot.w = transform.rotation.w;
		transform.rotation = playerRot;

		if(!onGround || !notOnSlope) {
			upMov -= grav;
		} else {
			upMov = -grav;
		}

		movDirec.x = 0f;
		movDirec.z = 0f;
	}

	protected virtual void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.gameObject.tag != "Transparent") {
			hitNormal = hit.normal;
			// notOnSlope = we're on ground level enough to walk on OR we're hitting a straight-up wall
			notOnSlope = Vector3.Angle(Vector3.up, hitNormal) <= cc.slopeLimit || Vector3.Angle(Vector3.up, hitNormal) >= 89;
			if(movDirec.y <= 0 && hit.point.y < transform.position.y + .9f) {
				// hit ground
				if(notOnSlope) onGround = true;
				//Debug.Log("Hit ground");
				// else if the hit point is from above and inside our radius (on top of head rather than on outer edge)
			} else if(hit.point.y > transform.position.y + 4f && Mathf.Sqrt(Mathf.Pow(transform.position.x - hit.point.x, 2f) + Mathf.Pow(transform.position.z - hit.point.z, 2f)) < 2f * transform.localScale.x) {
				// hit something going up
				upMov = Mathf.Min(0f, upMov);
				//Debug.Log("I hit my head!");
			}
		}
	}
}
