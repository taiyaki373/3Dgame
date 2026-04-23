using UnityEngine;

public class EnemyCharacter : Character {
    public int damage = 20;

    public void Attack(Character target) {
        if (!IsAlive) return;
        if (target != null) target.TakeDamage(damage);
    }
}