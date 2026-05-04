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

    public event System.Action OnInventoryChanged;

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
    }

    // Called when a skill objective completes (tree felled, ore mined)
    public void AddCoins(long amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke();
    }

    // Called by shop/equipment managers when the player buys something. Returns false if unaffordable.
    public bool SpendCoins(long amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        OnCoinsChanged?.Invoke();
        return true;
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