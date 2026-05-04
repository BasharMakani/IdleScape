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
        if (ToolManager.Instance == null) { Debug.LogError("[DebugTester] No ToolManager in scene. Add the ToolManager component to a GameObject."); yield break; }
        if (ArmorManager.Instance == null) { Debug.LogError("[DebugTester] No ArmorManager in scene. Add the ArmorManager component to a GameObject."); yield break; }
        if (nodeManager == null) { Debug.LogError("[DebugTester] No ResourceNodeManager in scene. Add it to a GameObject."); yield break; }

        // Cheat: top up coins + levels so every purchase succeeds
        GameManager.Instance.coins = 999_999_999;
        LevelManager.Instance.woodcuttingLevel = 50;
        LevelManager.Instance.miningLevel = 50;

        // Buy everything (silently, before we subscribe to events)
        foreach (var t in ToolManager.Instance.GetAllTools()) ToolManager.Instance.BuyTool(t);
        foreach (var a in ArmorManager.Instance.GetAllArmors()) ArmorManager.Instance.BuyArmor(a);

        // Auto-log whenever something changes � much easier than remembering to query state
        GameManager.Instance.OnCoinsChanged += () =>
            Debug.Log($"[Coins] {GameManager.Instance.coins}");
        GameManager.Instance.OnInventoryChanged += LogInventoryDelta;
        LevelManager.Instance.OnXPChanged += () =>
            Debug.Log($"[XP] WC lvl {LevelManager.Instance.woodcuttingLevel} ({LevelManager.Instance.woodcuttingXP}xp) | Mining lvl {LevelManager.Instance.miningLevel} ({LevelManager.Instance.miningXP}xp)");
        ToolManager.Instance.OnToolChanged += () =>
        {
            var axe = ToolManager.Instance.GetEquipped(ToolType.Axe);
            var pick = ToolManager.Instance.GetEquipped(ToolType.Pickaxe);
            Debug.Log($"[Tools] Axe={(axe != null ? axe.toolName : "none")} ({ToolManager.Instance.GetCurrentDamage(ActiveSkill.Woodcutting)} dmg) | Pickaxe={(pick != null ? pick.toolName : "none")} ({ToolManager.Instance.GetCurrentDamage(ActiveSkill.Mining)} dmg)");
        };
        ArmorManager.Instance.OnArmorChanged += () =>
        {
            var a = ArmorManager.Instance.GetEquipped();
            Debug.Log($"[Armor] {(a != null ? a.armorName : "none")} (x{ArmorManager.Instance.GetXPMultiplier()} XP)");
        };

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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameManager.Instance.SwapSkill();
            Debug.Log($"[Swap] Active skill is now {GameManager.Instance.currentSkill}");
            LogCurrentState();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipToolTier(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipToolTier(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipToolTier(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EquipToolTier(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) EquipToolTier(5);

        if (Input.GetKeyDown(KeyCode.Z)) EquipArmorTier(1);
        if (Input.GetKeyDown(KeyCode.X)) EquipArmorTier(2);
        if (Input.GetKeyDown(KeyCode.C)) EquipArmorTier(3);
        if (Input.GetKeyDown(KeyCode.V)) EquipArmorTier(4);
        if (Input.GetKeyDown(KeyCode.B)) EquipArmorTier(5);
    }

    void EquipToolTier(int tier)
    {
        ActiveSkill skill = GameManager.Instance.currentSkill;
        ToolType type = skill == ActiveSkill.Woodcutting ? ToolType.Axe : ToolType.Pickaxe;
        ToolData tool = ToolManager.Instance.GetAllTools().Find(t => t.type == type && t.tier == tier);
        if (tool != null) ToolManager.Instance.EquipTool(tool);
    }

    void EquipArmorTier(int tier)
    {
        ArmorData armor = ArmorManager.Instance.GetAllArmors().Find(a => a.tier == tier);
        if (armor != null) ArmorManager.Instance.EquipArmor(armor);
    }

    void LogCurrentState()
    {
        ActiveSkill skill = GameManager.Instance.currentSkill;
        Debug.Log($"[State] Skill={skill} | Damage={ToolManager.Instance.GetCurrentDamage(skill)} | XP mult x{ArmorManager.Instance.GetXPMultiplier()} | Coins={GameManager.Instance.coins}");
    }

    // Inspector right-click fallbacks (work regardless of Input System config).
    // Right-click the DebugTester component in the Inspector while in Play Mode to use these.
    [ContextMenu("Hit Node")]
    void CtxHitNode() { if (nodeManager != null) { nodeManager.MainClick(); Debug.Log($"[Click] Node HP: {nodeManager.currentHP}/{nodeManager.maxHP}"); } }
    [ContextMenu("Swap Skill")]
    void CtxSwapSkill() { GameManager.Instance.SwapSkill(); LogCurrentState(); }
    [ContextMenu("Equip Tool Tier 1")] void CtxTool1() => EquipToolTier(1);
    [ContextMenu("Equip Tool Tier 2")] void CtxTool2() => EquipToolTier(2);
    [ContextMenu("Equip Tool Tier 3")] void CtxTool3() => EquipToolTier(3);
    [ContextMenu("Equip Tool Tier 4")] void CtxTool4() => EquipToolTier(4);
    [ContextMenu("Equip Tool Tier 5")] void CtxTool5() => EquipToolTier(5);
    [ContextMenu("Equip Armor Tier 1")] void CtxArmor1() => EquipArmorTier(1);
    [ContextMenu("Equip Armor Tier 2")] void CtxArmor2() => EquipArmorTier(2);
    [ContextMenu("Equip Armor Tier 3")] void CtxArmor3() => EquipArmorTier(3);
    [ContextMenu("Equip Armor Tier 4")] void CtxArmor4() => EquipArmorTier(4);
    [ContextMenu("Equip Armor Tier 5")] void CtxArmor5() => EquipArmorTier(5);

    // Diff inventory to log just the item that was added on the most recent change
    private System.Collections.Generic.Dictionary<ItemID, int> lastInventory = new System.Collections.Generic.Dictionary<ItemID, int>();
    void LogInventoryDelta()
    {
        var inv = GameManager.Instance.inventory;
        foreach (var kvp in inv)
        {
            int prev = lastInventory.TryGetValue(kvp.Key, out int v) ? v : 0;
            if (kvp.Value != prev)
            {
                Debug.Log($"[Inventory] +{kvp.Value - prev} {kvp.Key} (total {kvp.Value})");
            }
        }
        lastInventory = new System.Collections.Generic.Dictionary<ItemID, int>(inv);
    }
}