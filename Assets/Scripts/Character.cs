using UnityEngine;
using System;

public class Character : MonoBehaviour {
    public string charName = "Char";
    public int maxHP = 100;
    [HideInInspector] public int currentHP;
    public bool IsAlive => currentHP > 0;
    public event Action<Character> OnDeath;

    void Awake() { currentHP = maxHP; }

    public void TakeDamage(int dmg) {
        if (!IsAlive) return;
        currentHP = Mathf.Max(0, currentHP - dmg);
        Debug.Log($"{charName} took {dmg} damage. HP={currentHP}/{maxHP}");
        if (currentHP == 0) {
            Debug.Log($"{charName} died.");
            OnDeath?.Invoke(this);
        }
    }
}