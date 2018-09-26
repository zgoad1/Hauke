using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class HaukePlayer : BattlePlayer {

	[SerializeField] private GameObject boomerangs;
	[SerializeField] private Image crosshair;

	private bool shifting;				// shifting from 3rd person to 1st (also true while in 1st person)
	private Color newColor;				// for crosshair alpha fading
	private int[] iAtkDamage;			// keep track of original attack damage so we can reset to it if it changes
	private Renderer r;					// for making the player flash for the charge effect
	private HaukeAtkHitbox0 ahb1;		// hitboxes for parts of the combo
	private AHBType2 ahb2;
	private HaukeAtkHitbox0 ahb3;
	private Boomerang[] realRangs;      // boomerangs that actually fly around and do damage
	
	protected override void Start() {
		r = GetComponent<FadeWhenClose>().r;    // this is usually the right renderer
		ahb1 = GetComponentInChildren<HaukeAtkHitbox0>();	// combo 1
		ahb2 = GetComponentInChildren<HaukeAtkHitbox2>();	// combo 2 (skip HAHB1, which is boomerang hitbox)
		ahb3 = GetComponentInChildren<HaukeAtkHitbox3>();	// combo 3
		realRangs = boomerangs.GetComponentsInChildren<Boomerang>();
		foreach(Boomerang b in realRangs) b.gameObject.SetActive(false);

		newColor = crosshair.color;
		
		iAtkDamage = new int[atkDamage.Length];
		for(int i = 0; i < atkDamage.Length; i++) iAtkDamage[i] = atkDamage[i];
		maxHp = 100;
		numWeapons = 2;

		// set emission to yellow for charge flash
		Color c = new Color(0, 0.4f, 0.5f);
		foreach(Material m in r.materials) {
			m.SetColor("_EmissionColor", c);
		}

		base.Start();
	}
	
	protected override void Update() {
		base.Update();

		#region Attack checks
		if(GetActiveWeapons().Count > 0) {
			// if LMB is pressed and we're not currently combo-ing and we have a weapon
			if(Input.GetButtonDown("Fire1") && !MTSBBI.AnyInArray(attacking)) {
				Debug.Log("Attacking");
				StartCoroutine("ComboAttack1");
			}

			if(Input.GetButtonDown("Fire2") && !MTSBBI.AnyInArray(attacking)) {
				StartCoroutine("ShiftPerspective");
			} else if(Input.GetButtonUp("Fire2")) {
				StopCoroutine("ShiftPerspective");
				if(!MTSBBI.AnyInArray(attacking)) {
					ThrowBoomerang();
				}
				shifting = false;
			}
		}
		#endregion

		#region First person shifting
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
		#endregion

		// increment stamina
		st += 0.15f;
	}

	protected bool IsInCombo() {
		return attacking[0] || attacking[1] || attacking[2];
	}

	protected override void Die() {
		Debug.Log("ded");
		gameObject.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	protected override IEnumerator Dodge() {
		if(st >= 20) {
			st -= 20;
			dodging = true;
			movDirec = (Mathf.Abs(movDirec.x) <= stopSpeed && Mathf.Abs(movDirec.z) <= stopSpeed) ? camTransform.forward * speed * 3f : movDirec.normalized * speed * 3f;
			transform.forward = movDirec;
			yield return new WaitForSeconds(0.3f);
			dodging = false;
		}
	}

	private IEnumerator ShiftPerspective() {
		yield return new WaitForSeconds(0.13f);
		shifting = true;
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

	// set the vector for transform.forward to lerp to
	// obsolete if we do strafing
	protected override void SetForwardTarget() {
		// Set forwardTarget to the camera's forward from this frame for a few moments, then set it back
		/*
		if(attacking[3]) {
			forwardTarget.x = camTransform.forward.x;
			if(Mathf.Abs(transform.forward.x - camTransform.forward.x) < 0.05f) {
				// turn the player a bit so they don't get stuck in one of those funky rotation lerps
				Debug.Log("Setting player's forward target");
				transform.forward = new Vector3(transform.forward.x + 0.5f, 0, transform.forward.z).normalized;
			}
			forwardTarget.z = camTransform.forward.z;
			forwardTarget = forwardTarget.normalized;
		} else {
		*/
			base.SetForwardTarget();
		//}
	}

	#region Attacks
	private void ThrowBoomerang() {
		if(GetActiveWeapons().Count == 0) anim.SetBool("hasWeapon", false);
		anim.SetTrigger("throwWeapon");
		StartCoroutine("BoomerangAttack");
		attacking[3] = true;
	}

	/*
	private void UseWeapon() {
		StartCoroutine("ComboAttack1");
		anim.SetTrigger("useWeapon");
		//hackCharged = false;
		StartCoroutine("ResetAtkDamage");
		attacking[0] = true;
	}
	*/
	
	private IEnumerator ComboAttack1() {
		int stCost = 10;
		if(st >= stCost) {                                              // if we can afford the stamina...
			Debug.Log("Performing attack1");
			attacking[0] = true;
			anim.SetTrigger("attack1");
			yield return new WaitForSeconds(0.2f);                      // wait for part of animation to finish
			ahb1.attacking = true;                                      // exert hitbox and damage enemies
			st -= stCost;                                               // take some stamina
			// [special effects]
			yield return new WaitForSeconds(0.3f);                      // wait for some more of animation
			bool keepGoing = false;
			for(int i = 0; i < 20; i++) {
				if(Input.GetButtonDown("Fire1")) {
					keepGoing = true;									// if we click again within 20 frames, do the next attack
					break;
				} else if(Input.GetButtonDown("Fire2")) {				// but if we press other buttons, break and perform the appropriate action
					StartCoroutine("ShiftPerspective");
					break;
				}
				yield return new WaitForSeconds(1/60f);
				Debug.Log("Accepting input after attack1");
			}
			if(keepGoing) StartCoroutine("ComboAttack2");
			attacking[0] = false;
		}
	}

	private IEnumerator ComboAttack2() {
		int stCost = 20;
		if(st >= stCost) {                          // if we can afford the stamina...
			Debug.Log("Performing attack2");
			attacking[1] = true;
			// [set animation trigger]
			// [move Hauke towards camera's forward]
			yield return new WaitForSeconds(0.1f);  // wait for part of animation
			ahb2.enabled = true;                    // begin exerting hitbox
			st -= stCost;
			// [special effects]
			yield return new WaitForSeconds(0.4f);  // wait some more
			bool keepGoing = false;                 // whether we'll keep doing attack2
			int maxFrames = 0;                      // how many frames we've been holding Fire1 at the end of the loop
			for(int i = 0; i < 20; i++) {
				if(Input.GetButton("Fire1")) {
					keepGoing = true;
					maxFrames++;
				} else {
					maxFrames = 0;
				}
				yield return new WaitForSeconds(1 / 60f);
				Debug.Log("Accepting input after attack2");
			}
			if(maxFrames >= 5 && Input.GetButton("Fire1")) {    // if we were holding LMB for a good amount of time, we probably want to...
				attacking[1] = false;
				StartCoroutine("ComboAttack3");                 // ...do the attack for that
			} else if(keepGoing) {                              // else if we clicked at all (and were probably tapping)...
				StartCoroutine("ComboAttack2");                 // ...keep doing this attack
			} else {
				attacking[1] = false;                           // else stop the combo
			}
		}
	}

	private IEnumerator ComboAttack3() {
		int stCost = 20;
		if(st >= stCost) {							// if we can afford the stamina...
			Debug.Log("Performing attack3");
			attacking[2] = true;
			float scale = 1;										// power and radius of attack
			StartCoroutine("ChargeFlash");
			while(Input.GetButton("Fire1")) {						// while holding LMB, increase the power and radius
				scale = Mathf.Min(2, scale + 1/10f);
				ahb3.transform.localScale = ahb3.iScale * scale;    // make hitbox bigger based on how long we held LMB (has to go here so it can detect trigger collisions)
				yield return new WaitForSeconds(0.1f);
			}
			// [set animation trigger]			
			yield return new WaitForSeconds(0.5f);                  // wait for part of animation
			ahb3.attacking = true;									// exert hitbox
			st -= stCost;
			cam.ScreenShake(0.3f + scale / 2f);
			// [special effects (use scale here)]
			StopCoroutine("ChargeFlash");			// stop flashing
			foreach(Material m in r.materials) {	// ensure our emission doesn't stay on
				m.DisableKeyword("_EMISSION");
			}
			attacking[2] = false;
		}
	}

	private IEnumerator BoomerangAttack() {
		if(st >= 5) {
			yield return new WaitForSeconds(0.15f);                             // wait for part of animation
			Boomerang b = Array.Find(realRangs, r => !r.gameObject.activeSelf); // find an inactive (actual) boomerang
			Debug.Log("realRangs[0]: " + realRangs[0]);
			b.gameObject.SetActive(true);                                       // activate it	// might need to use MTSBBI.SetActiveChildren here if this messes up
			b.hb.Begin();														// begin animation and hitbox
			GetActiveWeapons()[0].gameObject.SetActive(false);					// turn hand boomerang invisible
			st -= 5;
			yield return new WaitForSeconds(0.2f);	// wait a bit longer, then...
			attacking[3] = false;					// ...reenable attacking
		}
	}
	#endregion

	#region Animation nonsense
	public void AddThrowWeight() {
		StartCoroutine("AddThrowWeightCR");
	}

	public void RemoveThrowWeight() {
		//attacking[0] = false;   // reenable boomerang hack since near end of animation
		StartCoroutine("RemoveThrowWeightCR");
	}

	// these two are called by the throw animation
	private IEnumerator AddThrowWeightCR() {
		Debug.Log("Adding throw animation weight to layer: " + anim.GetLayerName(1));
		for(float i = 0; i < 4; i++) {
			yield return null;
			anim.SetLayerWeight(anim.GetLayerIndex("Upper Body"), (i + 1) / 4);
		}
		Debug.Log("Weight added, now: " + anim.GetLayerWeight(anim.GetLayerIndex("Upper Body")));
	}

	private IEnumerator RemoveThrowWeightCR() {
		Debug.Log("Removing throw animation weight");
		for(float i = 0; i < 8; i++) {
			yield return null;
			anim.SetLayerWeight(anim.GetLayerIndex("Upper Body"), 1 - (i + 1) / 8);
		}
		Debug.Log("Weight removed, now: " + anim.GetLayerWeight(anim.GetLayerIndex("Upper Body")));
		anim.SetLayerWeight(anim.GetLayerIndex("Upper Body"), 0.01f);   // Because Unity, if we set the animation's weight to 0 exactly, events won't trigger. So here, the weight would get locked at 0.
	}

	public void OnThrowAnimationFinish() {
		Debug.Log("Boomerang throw animation finished");
	}

	public override void CatchWeapon() {
		anim.SetBool("hasWeapon", true);
		foreach(HandWeapon w in handWeapons) {
			if(w.gameObject.activeSelf == false) {
				w.gameObject.SetActive(true);
				break;
			}
		}
																				// TODO: if both handboomerangs are inactive, set the RH one to active, else do the LH one
		//handBoomerang.gameObject.SetActive(true);
	}
	#endregion
}
