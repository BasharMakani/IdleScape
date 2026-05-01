using UnityEngine;
using System;
using System.Collections.Generic;

 public enum ActiveSkill { Woodcutting, Mining }
    // Item IDs for inventory management
    public enum ItemID
    {
    NormalWood,
    OakWood,
    WillowWood,
    MapleWood,
    MagicWood,

    CopperOre,
    IronOre,
    GoldOre,
    MithrilOre,
    RuniteOre
    }



public class GameManager : MonoBehaviour
{
   

    // Gamestate Trackers
    public ActiveSkill currentSkill = ActiveSkill.Woodcutting;
    public long coins = 0;
    public int woodcuttingLevel = 1;
    public int miningLevel = 1;
    public int woodcuttingXP = 0;
    public int miningXP = 0;

    public event System.Action OnInventoryChanged;
    public event System.Action OnXPChanged;


    // Listeners for invoking UIManager methods
    public event System.Action OnSkillSwapped;
    public event System.Action OnCoinsChanged;

    // Init GameManager instance
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coins = 0;
    }

    // Called directly from skill swap button, invokes UI manager flag
    public void SwapSkill()
    {
        currentSkill = currentSkill == ActiveSkill.Woodcutting ? ActiveSkill.Mining : ActiveSkill.Woodcutting;
        OnSkillSwapped?.Invoke();

        // DELETE ME ONCE CLICKER IS IMPLEMENTED
        
    }

    // Called when a skill objective completes (tree felled, ore mined)
    void AddCoins(long amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke();
    }
    // Inventory Management
    public Dictionary<ItemID, int> inventory = new Dictionary<ItemID, int>();
    //Adds specific amount of an item to the inventory 
    public void AddItem(ItemID itemID, int amount)
    {
    if (!inventory.ContainsKey(itemID))
    {
        inventory[itemID] = 0;
    }
    inventory[itemID] += amount;
    OnInventoryChanged?.Invoke();
    }
    
    //Adds xp to skills, handles level up logic
    public void AddWoodcuttingXP(int amount)
    {
    woodcuttingXP += amount;
    while (woodcuttingXP >= GetXPToNextLevel(woodcuttingLevel))
    {
        woodcuttingXP -= GetXPToNextLevel(woodcuttingLevel);
        woodcuttingLevel++;
    }
    OnXPChanged?.Invoke();
    }
        //Adds xp to skills, handles level up logic
    public void AddMiningXP(int amount)
    {
    miningXP += amount;
    while (miningXP >= GetXPToNextLevel(miningLevel))
    {
        miningXP -= GetXPToNextLevel(miningLevel);
        miningLevel++;
    }
    OnXPChanged?.Invoke();
    }
    //XP required to reach next level
    int GetXPToNextLevel(int level)
    {
    return level * 50; 
    }
    //Returns amount of an item in inventory, returns 0 if item not present
    public int GetItemAmount(ItemID itemID)
    {
    if (!inventory.ContainsKey(itemID))
    {
        return 0;
    }

    return inventory[itemID];
    }
   
}
