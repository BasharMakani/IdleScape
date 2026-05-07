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
    public int currentVanityId = 0; // Default is no helmet/hat
    public VanityOption[] vanityOptions;
    public event System.Action<int> OnVanityChanged;

    // Upgrade Stat Trackers
    public int damage = 0;
    public float XPMultiplier = 1.0f;
    public float CoinMultiplier = 1.0f; 


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
        coins = 0;
    }

    public int GetDamage(){
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

    // Called by shop/equipment managers when the player buys something. Returns false if unaffordable.
    public bool SpendCoins(long amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        OnCoinsChanged?.Invoke();
        return true;
    }

    public void PurchaseUpgrade(int upgradeId){

    }

    // Called from RefreshVanityDisplay from UiManager
    public void SetVanity(int id) {
        currentVanityId = id;
        OnVanityChanged?.Invoke(id);
    }


}

[System.Serializable]
public class VanityOption {
    public string name;
    public int id;
    public bool isPurchased;
    public Sprite vanitySprite;
}