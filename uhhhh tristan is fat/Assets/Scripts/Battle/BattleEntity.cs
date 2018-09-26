using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadeWhenClose))]
//[RequireComponent(typeof(Rigidbody))]
public class BattleEntity : MonoBehaviour {

	/* Anything that can partake in a battle, including the player, their companion,
	 * and enemies. All have HP, can take damage, and can die.
	 * 
	 * SET UPON START:
	 * - maxHp
	 * - attacking (array)
	 * - atkDamage (array & elements)
	 */

	private int mhp;
	protected int maxHp {
		get {
			return mhp;
		}
		set {
			mhp = value;
			hp = value;
		}
	}
	private int h;
	public int hp {
		get {
			return h;
		}
		set {
			h = Mathf.Clamp(value, 0, maxHp);
		}
	}
	[HideInInspector] public bool[] attacking;	// list of booleans for each attack button being pressed
	public int[] atkDamage;						// list of damage for each attack
	protected bool invincible = false;
	protected Vector3 verticalForce = new Vector3(0, 2000, 0);
	protected Animator anim;
	protected BattlePlayer player;
	protected Rigidbody rb;

	protected virtual void Start() {
		attacking = new bool[atkDamage.Length];
		rb = GetComponent<Rigidbody>();
	}

	protected virtual void Reset() {
		anim = GetComponent<Animator>();
		player = FindObjectOfType<BattlePlayer>();
	}

	public virtual void TakeDamage(int damage) {
		if(!invincible) {
			hp -= damage;
			if(hp == 0) Die();
			else StartCoroutine("GracePeriod");
		}
	}

	public virtual void Knockback(Vector3 force) { }

	protected virtual void Die() {
		gameObject.SetActive(false);
		// maybe some default smoke particles
	}

	private IEnumerator GracePeriod() {
		invincible = true;
		yield return new WaitForSeconds(0.35f);
		invincible = false;
	}
}
