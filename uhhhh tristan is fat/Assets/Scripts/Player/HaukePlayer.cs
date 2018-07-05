using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class HaukePlayer : BattlePlayer {
	
	[SerializeField] private GameObject boomerang;
	[SerializeField] private Image crosshair;

	#region Water bar variables
	private int w = 100;
	private int maxWater = 100;
	private int water {
		get {
			return w;
		}
		set {
			w = Mathf.Clamp(value, 0, maxWater);
		}
	}
	private WaterBar waterBar;
	private Vector3 wiScale;
	#endregion

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

		waterBar = FindObjectOfType<WaterBar>();
		wiScale = waterBar.transform.localScale;

		base.Start();
	}

	// Update is called once per frame
	protected override void Update() {
		base.Update();

		#region Attack checks
		if(Input.GetButtonDown("Fire1")) {
			StartCoroutine("ChargeHack");
		} else if(Input.GetButtonUp("Fire1")) {
			StopCoroutine("ChargeHack");
			StopCoroutine("ChargeFlash");
			foreach(Material m in r.materials) {
				m.DisableKeyword("_EMISSION");
			}
			if(hackCharged) {
				if(st >= 15) {
					attacking[0] = true;
					st -= 15;
				}

				// more/cooler effects

			} else {
				if(st >= 5) {
					attacking[0] = true;
					st -= 5;
				}

				// attack effects
			}

			hackCharged = false;
			StartCoroutine("ResetAtkDamage");
		}

		if(Input.GetButtonDown("Fire2")) {
			StartCoroutine("ShiftPerspective");
		} else if(Input.GetButtonUp("Fire2")) {
			StopCoroutine("ShiftPerspective");
			if(!attacking[1] && st >= 5) {
				if(!boomerang.gameObject.activeSelf) {
					MTSBBI.SetActiveChildren(boomerang.transform, true);
					boomerang.GetComponentInChildren<HaukeAtkHitbox1>().Begin();
				}
				attacking[1] = true;
				st -= 5;
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
		
		// set water bar
		wiScale.y = Mathf.Lerp(wiScale.y, (float) water / maxWater, lerpFac);
		waterBar.transform.localScale = wiScale;

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
			movDirec = (Mathf.Abs(movDirec.x) - stopSpeed <= 0 && Mathf.Abs(movDirec.z) - stopSpeed <= 0) ? camTransform.forward * speed * 3f : movDirec.normalized * speed * 3f;
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
}
