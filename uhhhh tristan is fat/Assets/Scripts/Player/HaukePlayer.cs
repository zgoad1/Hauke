using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class HaukePlayer : BattlePlayer {
	
	[SerializeField] private GameObject boomerang;
	[SerializeField] private Image crosshair;
	[SerializeField] private float jumpForce = 0.5f;
	
	// Water bar variables

	private bool jKey;
	private bool dodgeKey;
	private bool dodging = false;
	private bool shifting;  // shifting from 3rd person to 1st (also true while in 1st person)
	private Color newColor;	// for crosshair alpha fading
	private bool hackCharged = false;   // whether boomerang hack will do the cool version
	private int[] iAtkDamage;
	private Renderer r;     // for making the player flash for the charge effect
	//private Animator anim;

	// Use this for initialization
	protected override void Start() {
		attacking = new bool[2];
		atkDamage = new int[] { 35, 25 };   // hack, boomerang
		iAtkDamage = new int[atkDamage.Length];
		for(int i = 0; i < atkDamage.Length; i++) iAtkDamage[i] = atkDamage[i];
		maxHp = 100;
		
		newColor = crosshair.color;
		r = GetComponentInChildren<Renderer>();
		//anim = GetComponent<Animator>();

		// set emission to yellow for charge flash
		Color yellowey = new Color(0.3f, 0.3f, 0);
		foreach(Material m in r.materials) {
			m.SetColor("_EmissionColor", yellowey);
		}

		base.Start();
	}

	// Update is called once per frame
	protected override void Update() {
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
			foreach(Material m in r.materials) {
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
				Debug.Log("Dodging, setting CSJ to false");
			} else if(dodgeKey) {
				Debug.Log("Dodge failed. onGround = " + onGround + "\ncanStillJump = " + canStillJump);
			}
		}
		//if(jKey) Debug.LogWarning("Jumping\njKey = " + jKey + "\nonGround = " + onGround + "\ncanStillJump = " + canStillJump);
		//if(dodgeKey) Debug.LogWarning("Dodging\ndodgeKey = " + dodgeKey + "\nonGround = " + onGround);
		
		onGround = false;

		///////////////////					Yo, aren't further sets and checks of onGround obsolete because of ^ that?

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
			Debug.Log("Sliding on slope, setting CSJ to false");
		}
		cc.Move(movDirec);  // triggers collision detection
		anim.SetFloat("speed", movDirec.magnitude);
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
			Debug.Log("Jumping, setting CSJ to false");
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

	protected override void Die() {
		Debug.Log("ded");
		gameObject.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Grace period in which you can still jump after moving off of an edge
	/*
	private IEnumerator CanStillJump() {
		canStillJump = true;
		yield return new WaitForSeconds(0.2f);
		Debug.Log("Fell for 0.2 seconds, setting CSJ to false.");
		canStillJump = false;
	}
	*/

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
		foreach(Material m in r.materials) {
			m.EnableKeyword("_EMISSION");
		}
		yield return new WaitForSeconds(0.03f);
		foreach(Material m in r.materials) {
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
