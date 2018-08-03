using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class HaukeyPlayer : BattlePlayer {

	[SerializeField] private GameObject boomerang;
	[SerializeField] private Image crosshair;

	private bool shifting;  // shifting from 3rd person to 1st (also true while in 1st person)
	private Color newColor; // for crosshair alpha fading
	private bool hackCharged = false;   // whether boomerang hack will do the cool version
	private int[] iAtkDamage;
	private Renderer r;     // for making the player flash for the charge effect
	private HandWeapon handBoomerang;
							//private Animator anim;

	// Use this for initialization
	protected override void Start() {
		handBoomerang = GetComponentInChildren<HandWeapon>();
		attacking = new bool[2];
		atkDamage = new int[] { 35, 25 };   // hack, boomerang
		iAtkDamage = new int[atkDamage.Length];
		for(int i = 0; i < atkDamage.Length; i++) iAtkDamage[i] = atkDamage[i];
		maxHp = 100;

		newColor = crosshair.color;
		r = GetComponentInChildren<Renderer>();

		// set emission to yellow for charge flash
		Color yellowey = new Color(0.4f, 0.4f, 0);
		foreach(Material m in r.materials) {
			m.SetColor("_EmissionColor", yellowey);
		}

		base.Start();
	}

	// Update is called once per frame
	protected override void Update() {
		base.Update();

		#region Attack checks
		if(Input.GetButtonDown("Fire1") && !MTSBBI.AnyInArray(attacking)) {	// if we click and our boomerang isn't flying around
			StartCoroutine("ChargeHack");
		} else if(Input.GetButtonUp("Fire1") && !MTSBBI.AnyInArray(attacking)) {
			StopCoroutine("ChargeHack");
			StopCoroutine("ChargeFlash");
			foreach(Material m in r.materials) {
				m.DisableKeyword("_EMISSION");
			}
			if(hackCharged) {
				if(st >= 15) {
					st -= 15;
				}

				// more/cooler effects

			} else {
				if(st >= 5) {
					st -= 5;
				}

				// attack effects
			}

			UseWeapon();
		}

		if(Input.GetButtonDown("Fire2")) {
			StartCoroutine("ShiftPerspective");
		} else if(Input.GetButtonUp("Fire2")) {
			StopCoroutine("ShiftPerspective");
			if(!MTSBBI.AnyInArray(attacking) && st >= 5) {
				// set forward target transform
				ThrowBoomerang();
			}
			shifting = false;
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

	// set the vector for transform.forward to lerp to
	protected override void SetForwardTarget() {
		// Set forwardTarget to the camera's forward from this frame for a few moments, then set it back
		if(handBoomerang.gameObject.activeSelf && attacking[1]) {
			forwardTarget.x = camTransform.forward.x;
			if(Mathf.Abs(transform.forward.x - camTransform.forward.x) < 0.05f) {
				// turn the player a bit so they don't get stuck in one of those funky rotation lerps
				transform.forward = new Vector3(transform.forward.x + 0.5f, 0, transform.forward.z).normalized;
			}
			forwardTarget.z = camTransform.forward.z;
			forwardTarget = forwardTarget.normalized;
		} else {
			base.SetForwardTarget();
		}
	}

	#region Attack animation-related stuff
	private void ThrowBoomerang() {
		anim.SetBool("hasWeapon", false);
		anim.SetTrigger("throwWeapon");
		StartCoroutine("WaitForThrowAnim");
		attacking[1] = true;
	}

	private void UseWeapon() {
		StartCoroutine("WaitForHackAnim");
		anim.SetTrigger("useWeapon");
		hackCharged = false;
		StartCoroutine("ResetAtkDamage");
		attacking[0] = true;
	}

	private IEnumerator WaitForHackAnim() {
		yield return new WaitForSeconds(0.2f);
		GetComponentInChildren<HaukeAtkHitbox0>().attacking = true;
	}

	private IEnumerator WaitForThrowAnim() {
		yield return new WaitForSeconds(0.15f);
		if(!boomerang.gameObject.activeSelf) {
			MTSBBI.SetActiveChildren(boomerang.transform, true);
			boomerang.GetComponentInChildren<HaukeAtkHitbox1>().Begin();
		}
		// turn hand boomerang invisible
		handBoomerang.gameObject.SetActive(false);
		st -= 5;
	}

	public void AddThrowWeight() {
		StartCoroutine("AddThrowWeightCR");
	}

	public void RemoveThrowWeight() {
		attacking[0] = false;	// reenable boomerang hack since near end of animation
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
		anim.SetLayerWeight(anim.GetLayerIndex("Upper Body"), 0.01f);	// Because Unity, if we set the animation's weight to 0 exactly, events won't trigger. So here, the weight would get locked at 0.
	}

	public void OnThrowAnimationFinish() {
		Debug.Log("Boomerang throw animation finished");
	}

	public override void CatchWeapon() {
		anim.SetBool("hasWeapon", true);
		handBoomerang.gameObject.SetActive(true);
	}
	#endregion
}
