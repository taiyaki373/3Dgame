using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class BattleManager : MonoBehaviour {
    public static BattleManager Instance { get; private set; }

    public PlayerCharacter[] players; // assign 4 in inspector (front row)
    public EnemyCharacter[] enemies;  // assign 3 in inspector (back row)

    int currentPlayerIndex = 0;
    bool playerPhase = true;

    // Event fired when it's a player's turn: (player, playerIndex)
    public event Action<PlayerCharacter,int> OnPlayerTurnStarted;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        foreach (var p in players) if (p != null) p.OnDeath += OnCharacterDeath;
        foreach (var e in enemies) if (e != null) e.OnDeath += OnCharacterDeath;
        StartPlayerPhase();
    }

    void OnCharacterDeath(Character c) {
        CheckEndCondition();
    }

    void CheckEndCondition() {
        bool anyPlayerAlive = players.Any(p => p != null && p.IsAlive);
        bool anyEnemyAlive = enemies.Any(e => e != null && e.IsAlive);
        if (!anyPlayerAlive) {
            Debug.Log("Defeat: all players dead.");
            enabled = false;
        } else if (!anyEnemyAlive) {
            Debug.Log("Victory: all enemies dead.");
            enabled = false;
        }
    }

    void StartPlayerPhase() {
        playerPhase = true;
        currentPlayerIndex = GetNextAlivePlayerIndex(-1);
        PromptCurrentPlayer();
    }

    int GetNextAlivePlayerIndex(int fromIndex) {
        for (int i = fromIndex + 1; i < players.Length; i++) if (players[i] != null && players[i].IsAlive) return i;
        return -1;
    }

    void PromptCurrentPlayer() {
        if (currentPlayerIndex == -1) {
            StartCoroutine(EnemyPhaseRoutine());
            return;
        }
        var p = players[currentPlayerIndex];
        if (p == null || !p.IsAlive) { PlayerActionFinished(); return; }
        Debug.Log($"Player turn: {p.charName}. Choose Normal or AOE.");
        OnPlayerTurnStarted?.Invoke(p, currentPlayerIndex);
    }

    public void CommandNormal(int enemyIndex) {
        var player = players[currentPlayerIndex];
        var target = (enemyIndex >= 0 && enemyIndex < enemies.Length) ? enemies[enemyIndex] : null;
        player.NormalAttack(target);
    }

    public void CommandAOE() {
        var player = players[currentPlayerIndex];
        Character[] targets = enemies.Cast<Character>().ToArray();
        player.AreaAttack(targets);
    }

    public void PlayerActionFinished() {
        currentPlayerIndex = GetNextAlivePlayerIndex(currentPlayerIndex);
        PromptCurrentPlayer();
    }

    IEnumerator EnemyPhaseRoutine() {
        playerPhase = false;
        Debug.Log("Enemy phase start");
        yield return new WaitForSeconds(0.5f);
        foreach (var enemy in enemies) {
            if (enemy == null || !enemy.IsAlive) continue;
            var alivePlayers = players.Where(p => p != null && p.IsAlive).ToArray();
            if (alivePlayers.Length == 0) break;
            var target = alivePlayers[UnityEngine.Random.Range(0, alivePlayers.Length)];
            Debug.Log($"{enemy.charName} attacks {target.charName}");
            enemy.Attack(target);
            CheckEndCondition();
            yield return new WaitForSeconds(0.6f);
            if (!enabled) yield break;
        }
        yield return new WaitForSeconds(0.3f);
        StartPlayerPhase();
    }
}