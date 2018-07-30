using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayer : Controllable {

	// HP & stamina bars & other HUD variables

	protected HPBar hpBar;
	protected StBar stBar;
	protected Vector3 hpiScale;
	protected Vector3 stiScale;

	protected float lerpFac = 0.4f;

	protected override void Start() {
		base.Start();
		hpBar = FindObjectOfType<HPBar>();
		stBar = FindObjectOfType<StBar>();

		hpiScale = hpBar.transform.localScale;
		stiScale = stBar.transform.localScale;
	}

	protected override void Update() {
		hpiScale.x = Mathf.Lerp(hpiScale.x, (float) hp / maxHp, lerpFac);
		stiScale.x = Mathf.Lerp(stiScale.x, st / maxSt, lerpFac);

		hpBar.transform.localScale = hpiScale;
		stBar.transform.localScale = stiScale;

		base.Update();
	}

	public virtual void CatchWeapon() {

	}
}
