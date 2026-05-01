using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    // Master list of every tool that exists in the game
    private List<ToolData> tools = new List<ToolData>();
    // Tools the player has purchased (keyed by toolName)
    private HashSet<string> ownedTools = new HashSet<string>();
    // Currently equipped tool per type
    private ToolData equippedAxe;
    private ToolData equippedPickaxe;

    public event System.Action OnToolChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateTools();
        // Player starts with the tier 1 tools equipped (cost 0)
        ToolData starterAxe = GetTool("Bronze Axe");
        ToolData starterPickaxe = GetTool("Bronze Pickaxe");
        BuyTool(starterAxe);
        BuyTool(starterPickaxe);
        EquipTool(starterAxe);
        EquipTool(starterPickaxe);
    }

    // Initializes the tool list with every tool tier for both skills
    void CreateTools()
    {
        tools.Clear();
        tools.Add(new ToolData("Bronze Axe", ToolType.Axe, 1, 1, 0, 1));
        tools.Add(new ToolData("Iron Axe", ToolType.Axe, 2, 5, 50, 2));
        tools.Add(new ToolData("Steel Axe", ToolType.Axe, 3, 20, 500, 4));
        tools.Add(new ToolData("Mithril Axe", ToolType.Axe, 4, 40, 5000, 8));
        tools.Add(new ToolData("Rune Axe", ToolType.Axe, 5, 60, 50000, 16));

        tools.Add(new ToolData("Bronze Pickaxe", ToolType.Pickaxe, 1, 1, 0, 1));
        tools.Add(new ToolData("Iron Pickaxe", ToolType.Pickaxe, 2, 5, 50, 2));
        tools.Add(new ToolData("Steel Pickaxe", ToolType.Pickaxe, 3, 20, 500, 4));
        tools.Add(new ToolData("Mithril Pickaxe", ToolType.Pickaxe, 4, 40, 5000, 8));
        tools.Add(new ToolData("Rune Pickaxe", ToolType.Pickaxe, 5, 60, 50000, 16));
    }

    // Lookup helpers
    public ToolData GetTool(string name) => tools.Find(t => t.toolName == name);
    public List<ToolData> GetAllTools() => tools;
    public bool IsOwned(ToolData tool) => tool != null && ownedTools.Contains(tool.toolName);

    // Returns true if the player has the level and coins to buy this tool
    public bool CanBuy(ToolData tool)
    {
        if (tool == null || IsOwned(tool)) return false;
        if (GetPlayerLevelForTool(tool) < tool.requiredLevel) return false;
        return GameManager.Instance.coins >= tool.cost;
    }

    // Attempts to purchase a tool. Returns true on success.
    public bool BuyTool(ToolData tool)
    {
        if (!CanBuy(tool)) return false;
        if (!GameManager.Instance.SpendCoins(tool.cost)) return false;
        ownedTools.Add(tool.toolName);
        OnToolChanged?.Invoke();
        return true;
    }

    // Equips an owned tool into its matching slot
    public void EquipTool(ToolData tool)
    {
        if (!IsOwned(tool)) return;
        if (tool.type == ToolType.Axe) equippedAxe = tool;
        else equippedPickaxe = tool;
        OnToolChanged?.Invoke();
    }

    public ToolData GetEquipped(ToolType type)
    {
        return type == ToolType.Axe ? equippedAxe : equippedPickaxe;
    }

    // Returns the damage of the tool currently equipped for the given skill (defaults to 1 if none)
    public int GetCurrentDamage(ActiveSkill skill)
    {
        ToolData tool = skill == ActiveSkill.Woodcutting ? equippedAxe : equippedPickaxe;
        return tool != null ? tool.damage : 1;
    }

    int GetPlayerLevelForTool(ToolData tool)
    {
        ActiveSkill skill = tool.type == ToolType.Axe ? ActiveSkill.Woodcutting : ActiveSkill.Mining;
        return LevelManager.Instance.GetLevel(skill);
    }
}
