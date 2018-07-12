using UnityEngine;

public class Enemy : BattleEntity {

	/** Any enemy.
	 * 
	 * SET UPON START:
	 * - maxHp
	 * - attacking (array)
	 * - atkDamage (array & elements)
	 */

	protected bool frozen;

	public override void TakeDamage(int damage) {
		base.TakeDamage(frozen? damage * 2 : damage);
	}

	/** Exerts a hitbox and checks for player. Applies appropriate methods if hit.
	 */
	public virtual void Attack() { }

}
