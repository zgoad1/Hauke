using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class Player : Ally {

	[SerializeField] private Transform camTransform;
	[SerializeField] private GameObject boomerang;
	[SerializeField] private Image crosshair;
	[SerializeField] private float speed = 0.3f;
	[SerializeField] private float accel = 0.3f;
	[SerializeField] private float decel = 0.3f;
	[SerializeField] private float jumpForce = 0.5f;
	[SerializeField] private float grav = 0.03f;

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
	[HideInInspector] public bool canStillJump = true;

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
	private bool dodging = false;
	private float stopSpeed = 0.075f;
	private CameraControl cam;
	private float camDist;
	private bool shifting;  // shifting from 3rd person to 1st (also true while in 1st person)
	private Color newColor;
	private bool hackCharged = false;   // whether boomerang hack will do the cool version
	private int[] iAtkDamage;

	// Use this for initialization
	void Start() {
		attacking = new bool[2];
		atkDamage = new int[] { 35, 25 };   // hack, boomerang
		iAtkDamage = new int[atkDamage.Length];
		for(int i = 0; i < atkDamage.Length; i++) iAtkDamage[i] = atkDamage[i];
		maxHp = 100;

		ipos = transform.position;
		cc = GetComponent<CharacterController>();
		cam = FindObjectOfType<CameraControl>();
		camDist = cam.idistance;
		newColor = crosshair.color;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// set emission to yellow for charge flash
		Color yellowey = new Color(0.3f, 0.3f, 0);
		foreach(Material m in GetComponent<Renderer>().materials) {
			m.SetColor("_EmissionColor", yellowey);
		}
	}

	// Update is called once per frame
	void Update() {
		//controls
		rightKey = Input.GetAxisRaw("Horizontal");
		fwdKey = Input.GetAxisRaw("Vertical");
		jKey = Input.GetButtonDown("Jump") ? true : jKey;
		dodgeKey = Input.GetButtonDown("Run") ? true : dodgeKey;
		//attacking[0] = Input.GetButtonDown("Fire1") ? true : attacking[0];
		//attacking[1] = Input.GetButtonUp("Fire2")? true : attacking[1];

		if(Input.GetButtonDown("Fire1")) {
			StartCoroutine("ChargeHack");
		} else if(Input.GetButtonUp("Fire1")) {
			StopCoroutine("ChargeHack");
			StopCoroutine("ChargeFlash");
			foreach(Material m in GetComponent<Renderer>().materials) {
				m.DisableKeyword("_EMISSION");
			}
			attacking[0] = true;

			// attack effects
			if(hackCharged) {
				// more/cooler effects
			}

			hackCharged = false;
			StartCoroutine("ResetAtkDamage");
		}

		if(Input.GetButtonDown("Fire2")) {
			StartCoroutine("ShiftPerspective");
		} else if(Input.GetButtonUp("Fire2")) {
			StopCoroutine("ShiftPerspective");
			attacking[1] = true;
			shifting = false;
		}

		// zoom into first person view upon right click hold
		if(shifting) {
			cam.SetFirstPerson(true);
			cam.idistance = Mathf.Lerp(cam.idistance, 0.2f, 0.2f);
			newColor.a = Mathf.Lerp(newColor.a, 0.3f, 0.2f);
			crosshair.color = newColor;
		} else {
			cam.SetFirstPerson(false);
			cam.idistance = Mathf.Lerp(cam.idistance, camDist, 0.2f);
			newColor.a = Mathf.Lerp(newColor.a, 0f, 0.2f);
			crosshair.color = newColor;
		}

		// activate boomerang upon right click release
		if(attacking[1]) {
			if(!boomerang.gameObject.activeSelf) {
				MTSBBI.SetActiveChildren(boomerang.transform, true);
				boomerang.GetComponentInChildren<HaukeAtkHitbox1>().Begin();
			}
		}

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

		// vvv STUFF THAT USED TO BE IN FIXEDUPDATE vvv

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
		transform.forward = Vector3.Lerp(transform.forward, movDirec, 0.6f);
		playerRot.y = transform.rotation.y;
		playerRot.w = transform.rotation.w;
		transform.rotation = playerRot;

		// jumping & falling
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
		jKey = false;       // keep these true after they're pressed until FixedUpdate is called
		dodgeKey = false;

		if(!dodging) {
			movDirec.x = 0f;
			movDirec.z = 0f;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.gameObject.tag != "Transparent") {
			hitNormal = hit.normal;
			// notOnSlope = we're on ground level enough to walk on OR we're hitting a straight-up wall
			notOnSlope = Vector3.Angle(Vector3.up, hitNormal) <= cc.slopeLimit || Vector3.Angle(Vector3.up, hitNormal) >= 89;
			if(movDirec.y <= 0 && hit.point.y < transform.position.y + .9f) {
				// hit ground
				if(notOnSlope) onGround = true;
				//Debug.Log("Hit ground");
				// else if the hit point is from above and inside our radius (on top of head rather than on outer edge)
			} else if(hit.point.y > transform.position.y + 4f && Mathf.Sqrt(Mathf.Pow(transform.position.x - hit.point.x, 2) + Mathf.Pow(transform.position.z - hit.point.z, 2)) < transform.localScale.x) {
				// hit something going up
				upMov = Mathf.Min(0f, upMov);
				Debug.Log("I hit my head!");
			}
		}
	}

	protected override void Die() {
		Debug.Log("ded");
		gameObject.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Grace period in which you can still jump after moving off of an edge
	private IEnumerator CanStillJump() {
		canStillJump = true;
		yield return new WaitForSeconds(0.2f);
		canStillJump = false;
	}

	private IEnumerator Dodge() {
		dodging = true;
		movDirec = (Mathf.Abs(movDirec.x) - stopSpeed <= 0 && Mathf.Abs(movDirec.z) - stopSpeed <= 0) ? camTransform.forward * speed * 3f : movDirec.normalized * speed * 3f;
		transform.forward = movDirec;
		yield return new WaitForSeconds(0.3f);
		dodging = false;
	}

	private IEnumerator ShiftPerspective() {
		yield return new WaitForSeconds(0.13f);
		shifting = true;
	}

	private IEnumerator ChargeHack() {
		yield return new WaitForSeconds(1.2f);
		hackCharged = true;
		atkDamage[0] *= 2;
		StartCoroutine("ChargeFlash");
	}

	private IEnumerator ChargeFlash() {
		foreach(Material m in GetComponent<Renderer>().materials) {
			m.EnableKeyword("_EMISSION");
		}
		yield return new WaitForSeconds(0.03f);
		foreach(Material m in GetComponent<Renderer>().materials) {
			m.DisableKeyword("_EMISSION");
		}
		yield return new WaitForSeconds(0.1f);
		StartCoroutine("ChargeFlash");
	}

	private IEnumerator ResetAtkDamage() {
		yield return 1;
		for(int i = 0; i < atkDamage.Length; i++) atkDamage[i] = iAtkDamage[i];
	}
}
