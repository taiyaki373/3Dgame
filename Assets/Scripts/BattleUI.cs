using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {
    public Button normalButton;
    public Button aoeButton;
    public Button[] enemyButtons; // one per enemy, assign in inspector

    int selectedEnemy = 0;

    void Start() {
        if (BattleManager.Instance != null) BattleManager.Instance.OnPlayerTurnStarted += OnPlayerTurnStarted;
        SetupButtons();
        DisableActionButtons();
    }

    void OnDestroy() {
        if (BattleManager.Instance != null) BattleManager.Instance.OnPlayerTurnStarted -= OnPlayerTurnStarted;
    }

    void SetupButtons() {
        for (int i = 0; i < enemyButtons.Length; i++) {
            int idx = i;
            enemyButtons[i].onClick.AddListener(() => OnEnemyButton(idx));
        }
        normalButton.onClick.AddListener(OnNormalButton);
        aoeButton.onClick.AddListener(OnAOEButton);
    }

    void OnPlayerTurnStarted(PlayerCharacter player, int playerIndex) {
        // enable UI so player can choose
        EnableActionButtons();
        // select first alive enemy by default
        selectedEnemy = GetFirstAliveEnemyIndex();
        HighlightSelectedEnemy();
        Debug.Log($"UI: player {player.charName} turn. Selected enemy {selectedEnemy}");
    }

    int GetFirstAliveEnemyIndex() {
        var enemies = BattleManager.Instance.enemies;
        for (int i = 0; i < enemies.Length; i++) if (enemies[i] != null && enemies[i].IsAlive) return i;
        return 0;
    }

    void HighlightSelectedEnemy() {
        // simple visual: toggle interactable state
        for (int i = 0; i < enemyButtons.Length; i++) {
            var colors = enemyButtons[i].colors;
            colors.normalColor = (i == selectedEnemy) ? Color.green : Color.white;
            enemyButtons[i].colors = colors;
        }
    }

    void OnEnemyButton(int idx) {
        selectedEnemy = idx;
        HighlightSelectedEnemy();
    }

    void OnNormalButton() {
        DisableActionButtons();
        BattleManager.Instance.CommandNormal(selectedEnemy);
    }

    void OnAOEButton() {
        DisableActionButtons();
        BattleManager.Instance.CommandAOE();
    }

    void EnableActionButtons() {
        normalButton.interactable = true;
        aoeButton.interactable = true;
        foreach (var b in enemyButtons) if (b != null) b.interactable = true;
    }

    void DisableActionButtons() {
        normalButton.interactable = false;
        aoeButton.interactable = false;
        foreach (var b in enemyButtons) if (b != null) b.interactable = false;
    }
}