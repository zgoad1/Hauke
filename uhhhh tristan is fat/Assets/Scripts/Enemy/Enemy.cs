using UnityEngine;

public class Enemy : BattleEntity {

	/** Any enemy.
	 * 
	 * SET UPON START:
	 * - maxHp
	 * - attacking (array)
	 * - atkDamage (array & elements)
	 */

	/** Exerts a hitbox and checks for player. Applies appropriate methods if hit.
	 */
	public virtual void Attack() { }

}
