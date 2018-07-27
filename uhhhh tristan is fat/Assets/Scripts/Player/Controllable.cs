using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class Controllable : Ally {

	// NOTE: COLLISION DETECTION MUST BE CONTINUOUS or animations will be wacky
	#region Variables
	[SerializeField] protected float speed = 0.225f;
	[SerializeField] protected float accel = 0.175f;
	[SerializeField] protected float decel = 0.2f;
	[SerializeField] protected float grav = 0.03f;
	[SerializeField] private float jumpForce = 0.5f;

	private bool og = false;    // whether Percy can jump
	[HideInInspector]
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
	[HideInInspector] public Vector3 movDirec = Vector3.zero;    // direction of movement

	protected Transform camTransform;
	protected CharacterController cc;
	protected Vector3 ipos;   // keeps track of starting position so we can return
	protected float rightKey;
	protected float fwdKey;
	protected bool jKey;
	protected float rightMov = 0f;
	protected float fwdMov = 0f;
	protected float upMov = 0f;
	protected bool dodgeKey;
	protected bool dodging = false;
	protected Quaternion playerRot = new Quaternion(0f, 0f, 0f, 0f);
	protected Vector3 hitNormal = Vector3.zero;
	protected bool notOnSlope = false;
	protected float stopSpeed = 0.075f;
	protected CameraControl cam;
	protected float camDist;
	protected Animator anim;
	[HideInInspector] public List<GameObject> interactables = new List<GameObject>();
	protected List<Door> doors = new List<Door>();
	[HideInInspector] public bool canStillJump = false; // whether we're in the grace period in which we can still jump after walking off an edge
	protected bool facing = false;    // whether we're facing an interactable
	protected Transform facingTransform;
	protected Quaternion iRotation;
	public bool readInput = true;
	protected Vector3 prevPosition;
	#endregion

	protected virtual void Reset() {
		camTransform = FindObjectOfType<MainCamera>().transform;
		cc = GetComponent<CharacterController>();
		cam = FindObjectOfType<CameraControl>();
		anim = GetComponent<Animator>();
		iRotation = head.transform.localRotation;
		Transform h = Array.Find(transform.GetComponentsInChildren<Transform>(), t => t.gameObject.name == "Head");
		if(h.GetComponent<NPCHead>() == null) head = h.gameObject.AddComponent<NPCHead>();
		else head = h.GetComponent<NPCHead>();
		head.body = transform;
	}

	// Use this for initialization
	protected override void Start() {
		base.Start();

		Reset();

		ipos = transform.position;
		prevPosition = transform.position;
		camDist = cam.idistance;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Update is called once per frame
	protected virtual void Update() {

		//controls
		rightKey = Input.GetAxisRaw("Horizontal");
		fwdKey = Input.GetAxisRaw("Vertical");
		if(Mathf.Abs(rightKey) == 1 && Mathf.Abs(fwdKey) == 1) {
			rightKey = Mathf.Sign(rightKey) * 0.707f;
			fwdKey = Mathf.Sign(fwdKey) * 0.707f;
		}
		jKey = Input.GetButtonDown("Jump") ? true : jKey;
		dodgeKey = Input.GetButtonDown("Run") ? true : dodgeKey;

		#region Set move directions

		// change forward's y to 0 then normalize, in case the camera is pointed down or up
		Vector3 tempForward = camTransform.forward;
		tempForward.y = 0f;

		if(!dodging && readInput) {
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

			// get movement direction vector
			movDirec = tempForward.normalized * fwdMov + camTransform.right.normalized * rightMov;
			//anim.SetFloat("speed", movDirec.magnitude);

			if(dodgeKey && (onGround || canStillJump)) {
				StartCoroutine("Dodge");
				canStillJump = false;
				//Debug.Log("Dodging, setting CSJ to false");
			} else if(dodgeKey) {
				//Debug.Log("Dodge failed. onGround = " + onGround + "\ncanStillJump = " + canStillJump);
			}
		}
		#endregion

		#region Pause
			if(Input.GetButtonDown("Pause")) {
			if(Cursor.lockState != CursorLockMode.Locked) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			} else {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
		#endregion

		#region Calculate movement
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
			canStillJump = false;
			//Debug.Log("Sliding");
		}
		cc.Move(movDirec);  // T R I G G E R S   C O L L I S I O N   D E T E C T I O N  (AND CAN SET ONGROUND TO TRUE)

		// speed is the distance from where we were last frame to where we are now
		//Debug.Log("MovDirec: " + movDirec);
		float movDist = Vector3.Distance(prevPosition, transform.position);
		if(movDist < 0.05) {
			transform.position = prevPosition;
		}
		anim.SetFloat("speed", movDist);
		movDirec = (transform.position - prevPosition).normalized;
		movDirec.y = 0;
		prevPosition = transform.position;
		//Debug.Log("speed: " + anim.GetFloat("speed") + "\nmovDirec: " + movDirec);
		transform.forward = Vector3.Lerp(transform.forward, movDirec, 0.4f);
		playerRot.y = transform.rotation.y;
		playerRot.w = transform.rotation.w;
		transform.rotation = playerRot;

		// jumping & falling
		if(jKey && (onGround || canStillJump) && !dodging) {
			onGround = false;
			//Debug.LogWarning("Jumping\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
			upMov = jumpForce;
			canStillJump = false;
		} else if(!onGround || !notOnSlope) {
			upMov -= grav;
			//Debug.Log("Increasing gravity: " + upMov);
			if(jKey) Debug.LogWarning("Falling\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
		} else {
			upMov = -grav;
			if(jKey) Debug.LogWarning("Apex\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
		}
		jKey = false;       // keep these true after they're pressed until FixedUpdate is called
		dodgeKey = false;

		if(!dodging) {
			movDirec.x = 0f;
			movDirec.z = 0f;
		}
		#endregion

		// Update doors
		foreach(Door d in doors) {
			d.SetDistance(Vector3.Distance(transform.position, d.transform.position));
		}

		// Interacting
		if(readInput && Input.GetButtonDown("Fire1") && !FindObjectOfType<DialogueBox>().enabled && interactables.Count != 0) {
			Interact();
			Debug.Log("Interacting");
		}
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
			Door door = hit.gameObject.GetComponent<Door>();
			if(door != null) {
				//Debug.Log("Collision. Opening door.");
				door.Open();
			}
		}
	}

	#region Movement functions
	protected virtual IEnumerator Dodge() {
		yield return null;
	}

	// Grace period in which you can still jump after moving off of an edge
	protected virtual IEnumerator CanStillJump() {
		canStillJump = true;
		yield return new WaitForSeconds(0.2f);
		//Debug.Log("Fell for 0.2 seconds, setting CSJ to false.");
		canStillJump = false;
	}
	#endregion

	#region Interactable / Door functions
	protected void Interact(Interactable i) {
		Debug.Log("Interacting");
		if(i.zoomIn) cam.ZoomIn(i);
		if(i is NPC) {
			TurnBody(((NPC)i).head.transform);
			// head turning is handled by the NPC
		} else {
			TurnBody(i.transform);
			// head turning is handled by the interactable
		}
		i.Interact();
	}

	public void Interact() {
		if(interactables.Count > 0) Interact(MTSBBI.Closest(gameObject, interactables).GetComponent<Interactable>());
		else Debug.LogWarning("Tried to start an interaction with no one nearby");
	}

	public void AddInteractable(Interactable toAdd) {
		interactables.Add(toAdd.gameObject);
	}

	public void RemoveInteractable(Interactable toRemove) {
		interactables.Remove(toRemove.gameObject);
	}

	public void AddDoor(Door toAdd) {
		doors.Add(toAdd);
	}

	public void RemoveDoor(Door toRemove) {
		doors.Remove(toRemove);
	}
	#endregion

	#region SmoothTurning
	// SmoothTurn the body along the Y axis to face a target
	private void TurnBody(Transform target) {
		Quaternion oldRot = transform.localRotation;
		MTSBBI.LookAtXYZ(transform, target.transform, 2, 1);
		Quaternion newRot = transform.localRotation;
		transform.rotation = oldRot;
		SmoothTurn(transform, newRot);
	}

	// Slerp to a new rotation Quaternion
	public void SmoothTurn(Transform t, Quaternion q) {
		IEnumerator cr = SmoothTurnCR(t, q, 0.2f);
		StartCoroutine(cr);
	}

	// This has to go in this class because Unity sucks
	public static IEnumerator SmoothTurnCR(Transform t, Quaternion q, float lerpFac) {
		for(int i = 0; i < 60; i++) {
			t.localRotation = Quaternion.Slerp(t.localRotation, q, lerpFac);
			yield return null;
		}
	}
	#endregion

	public void Pause() {
		readInput = false;
		movDirec = Vector3.zero;
		rightMov = 0;
		fwdMov = 0;
		anim.SetFloat("speed", 0);
	}

	public void Unpause() {
		readInput = true;
	}
}
