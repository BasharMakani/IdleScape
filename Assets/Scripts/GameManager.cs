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
    MagicWood
}

public class GameManager : MonoBehaviour
{

    // Gamestate Trackers
    public ActiveSkill currentSkill = ActiveSkill.Woodcutting;
    public long coins = 0;

    // Vanity Trackers
    public VanityOption[] vanityOptions;
    public int currentVanityId = 0; // Default is no helmet/hat
    public event System.Action<int> OnVanityChanged;

    // Upgrade Stat Trackers
    public Upgrade[] upgrades;
    public event System.Action OnUpgradePurchased;
    public float damage = 0f;
    public float XPMultiplier = 1f;
    public float CoinMultiplier = 1f; 
    private int requiredLevelIncrease = 2;


    // Listeners for invoking UIManager methods
    public event System.Action OnCoinsChanged;

    // Init GameManager instance
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coins = 0; // Default is 0 coins
        damage = 1f; // Default is 1 damage per hit
    }

    public float GetDamage(){
        return damage;
    }

    public float GetXPMultiplier(){
        return XPMultiplier;
    }

    public float GetCoinMultiplier(){
        return CoinMultiplier;
    }

    // Called when a skill objective completes (tree felled, ore mined)
    public void AddCoins(long amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke();
    }

    // Called by UI Manager when the player buys something
    public void SpendCoins(long amount)
    {
        coins -= amount;
        OnCoinsChanged?.Invoke();
    }

    // Adds stat boost from upgrade and updates next upgrade's values
    public void PurchaseUpgrade(int upgradeId) 
    {
        Upgrade upgrade = upgrades[upgradeId];
        SpendCoins(upgrade.cost);
        
        upgrade.cost *= (long)upgrade.costMultiplier;
        upgrade.requiredLevel += requiredLevelIncrease;

        if (upgradeId == 0) damage += upgrade.statIncrease;
        if (upgradeId == 1) XPMultiplier += upgrade.statIncrease;
        if (upgradeId == 2) CoinMultiplier += upgrade.statIncrease;

        OnUpgradePurchased?.Invoke();
    }

    // Called from RefreshVanityDisplay from UiManager
    public void SetVanity(int id) {
        currentVanityId = id;
        OnVanityChanged?.Invoke(id);
    }


}

[System.Serializable]
public class Upgrade {
    public string name;
    public int id;
    public long cost;
    public int requiredLevel;
    public float costMultiplier;
    public float statIncrease;
}

[System.Serializable]
public class VanityOption {
    public string name;
    public int id;
    public long cost;
    public bool isPurchased;
    public Sprite vanitySprite;
    public GameObject prefab;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
}

