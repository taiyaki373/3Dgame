using UnityEngine;

public class PlayerCharacter : Character {
    public int normalDamage = 30;
    public int aoeDamage = 15;

    // UI calls these when player chooses an action
    public void NormalAttack(Character target) {
        if (!IsAlive) return;
        if (target != null) target.TakeDamage(normalDamage);
        BattleManager.Instance.PlayerActionFinished();
    }

    public void AreaAttack(Character[] targets) {
        if (!IsAlive) return;
        foreach (var t in targets) {
            if (t != null && t.IsAlive) t.TakeDamage(aoeDamage);
        }
        BattleManager.Instance.PlayerActionFinished();
    }
}