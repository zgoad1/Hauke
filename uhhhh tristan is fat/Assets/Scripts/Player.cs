using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField] private Transform camTransform;
	[SerializeField] private float speed = 0.3f;
	[SerializeField] private float accel = 0.3f;
	[SerializeField] private float decel = 0.3f;
	[SerializeField] private float jumpForce = 3f;
	[SerializeField] private float grav = 0.2f;

	public bool onGround = false;      // whether Percy can jump

	private CharacterController cc;
	private Rigidbody rb;
	private AudioManager am;
	private CameraControl camControl;
	private Vector3 ipos;   // keeps track of starting position so we can return
	private float rightKey;
	private float fwdKey;
	private bool jKey;
	private Vector3 movDirec = Vector3.zero;    // direction of movement, components set to the below 3
	private float rightMov = 0f;
	private float fwdMov = 0f;
	private float upMov = 0f;
	private Quaternion playerRot = new Quaternion(0f, 0f, 0f, 0f);
	private Vector3 hitNormal = Vector3.zero;
	private bool notOnSlope = false;
	private bool running = false;

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
		rb = GetComponent<Rigidbody>();
		am = FindObjectOfType<AudioManager>();
		camControl = camTransform.GetComponent<CameraControl>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update() {
		//controls
		rightKey = Input.GetAxisRaw("Horizontal");
		fwdKey = Input.GetAxisRaw("Vertical");
		jKey = Input.GetButtonDown("Jump");    // sometimes this is inaccurate - doesn't update when it should. Updates when another button is pressed though, so it works in motion. Close enough
		running = Input.GetButton("Run");	// use this as more of a dodge

		// change forward's y to 0 then normalize, in case the camera is pointed down or up
		Vector3 tempForward = camTransform.forward;
		tempForward.y = 0f;

		// speed up / slow down
		int moveFac = (running)? 2 : 1;
		if(rightKey != 0) {
			rightMov = Mathf.Lerp(rightMov, (rightKey * speed * moveFac), accel);
		} else {
			rightMov = Mathf.Lerp(rightMov, 0f, decel);
		}
		if(fwdKey != 0) {
			fwdMov = Mathf.Lerp(fwdMov, (fwdKey * speed * moveFac), accel);
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
		onGround = false;
	}

	// called once per physics update
	void FixedUpdate() {
		// calculate movement
		movDirec.y = upMov;
		//Character sliding of surfaces
		float slideFriction = 0.09f;
		if(!notOnSlope) {
			movDirec.x += (1f - hitNormal.y) * hitNormal.x * (1f - slideFriction);
			movDirec.z += (1f - hitNormal.y) * hitNormal.z * (1f - slideFriction);
			hitNormal = Vector3.zero;
		}
		cc.Move(movDirec);  // triggers collision detection
		notOnSlope = (Vector3.Angle(Vector3.up, hitNormal) <= cc.slopeLimit);
		transform.forward = Vector3.Lerp(transform.forward, movDirec, 0.6f);
		playerRot.y = transform.rotation.y;
		playerRot.w = transform.rotation.w;
		transform.rotation = playerRot;
		// jumping
		if(jKey && onGround) {
			onGround = false;
			Debug.Log("Left ground");
			upMov = jumpForce;
		} else if(!onGround || !notOnSlope) {
			upMov -= grav;
			Debug.Log("Increasing gravity: " + upMov);
		} else {
			upMov = -grav;
		}

		movDirec.x = 0f;
		movDirec.z = 0f;
	}

	void OnControllerColliderHit(ControllerColliderHit other) {
		if(other.gameObject.tag != "Transparent") {
			hitNormal = other.normal;
			if(movDirec.y <= 0 && other.point.y < transform.position.y - 1f) {
				// hit ground
				onGround = true;
				Debug.Log("Hit ground");
			} else if(other.point.y > transform.position.y - 0.5f) {
				// hit something going up
				upMov = Mathf.Min(0f, upMov);
				//Debug.Log("I hit my head!");
			} else {
				// sliding along a slope while falling
				// x/z should be proportional to y (if you fall on a slope fast you'll roll fast)
				// you CAN have movement code here
				//float slideFriction = 0.3f;
				//cc.Move(new Vector3((1f - other.normal.y) * other.normal.x * (1f - slideFriction), upMov, (1f - other.normal.y) * other.normal.z * (1f - slideFriction)));
				//movDirec.x += (1f - other.normal.y) * other.normal.x * (1f - slideFriction);
				//movDirec.z += (1f - other.normal.y) * other.normal.z * (1f - slideFriction);
				//Debug.Log("Sliding");
			}
		}
	}
}
