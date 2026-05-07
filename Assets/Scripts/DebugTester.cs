using UnityEngine;
using System.Collections;

// Drop this on any GameObject in the scene to enable keyboard testing of the gameplay loop.
// Hotkeys:
//   Space          hit the current resource node
//   Tab            swap active skill (Woodcutting <-> Mining)
//   1..5           equip tool tier 1-5 for the active skill (axe if WC, pickaxe if Mining)
//   Z/X/C/V/B      equip armor tier 1-5
public class DebugTester : MonoBehaviour
{
    private ResourceNodeManager nodeManager;

    void Start()
    {
        // Defer setup one frame so every other manager's Start() has run and populated its data
        StartCoroutine(SetupAfterFrame());
    }

    IEnumerator SetupAfterFrame()
    {
        yield return null;

        nodeManager = FindFirstObjectByType<ResourceNodeManager>();

        // Verify all required managers are in the scene before doing anything
        if (GameManager.Instance == null) { Debug.LogError("[DebugTester] No GameManager in scene. Add the GameManager component to a GameObject."); yield break; }
        if (LevelManager.Instance == null) { Debug.LogError("[DebugTester] No LevelManager in scene. Add the LevelManager component to a GameObject."); yield break; }

        if (nodeManager == null) { Debug.LogError("[DebugTester] No ResourceNodeManager in scene. Add it to a GameObject."); yield break; }

        // Cheat: top up coins + levels so every purchase succeeds
        GameManager.Instance.coins = 999_999_999;
        LevelManager.Instance.woodcuttingLevel = 50;
        LevelManager.Instance.miningLevel = 50;

        // Auto-log whenever something changes � much easier than remembering to query state
        GameManager.Instance.OnCoinsChanged += () =>
            Debug.Log($"[Coins] {GameManager.Instance.coins}");
        LevelManager.Instance.OnXPChanged += () =>
            Debug.Log($"[XP] WC lvl {LevelManager.Instance.woodcuttingLevel} ({LevelManager.Instance.woodcuttingXP}xp) | Mining lvl {LevelManager.Instance.miningLevel} ({LevelManager.Instance.miningXP}xp)");


        Debug.Log("=== DebugTester ready ===\n" +
                  "Space: hit node | Tab: swap skill\n" +
                  "1-5: equip tool tier | Z/X/C/V/B: equip armor tier 1-5");
        LogCurrentState();
    }

    void Update()
    {
        if (nodeManager == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            nodeManager.MainClick();
            Debug.Log($"[Click] Node HP: {nodeManager.currentHP}/{nodeManager.maxHP}");
        }
    }


    void LogCurrentState()
    {
        ActiveSkill skill = GameManager.Instance.currentSkill;
        Debug.Log($"[State] Skill={skill} | Coins={GameManager.Instance.coins}");
    }

}