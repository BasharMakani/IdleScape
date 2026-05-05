using UnityEngine;
using System.Collections.Generic;

public class ArmorManager : MonoBehaviour
{
    public static ArmorManager Instance { get; private set; }

    // Master list of every armor piece in the game
    private List<ArmorData> armors = new List<ArmorData>();
    // Armors the player has purchased (keyed by armorName)
    private HashSet<string> ownedArmors = new HashSet<string>();
    // Single equipped armor slot (universal � boost applies to both skills)
    private ArmorData equippedArmor;

    public event System.Action OnArmorChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateArmors();
        // Player starts with the tier 1 armor equipped (cost 0)
        ArmorData starter = GetArmor("Cloth Robes");
        BuyArmor(starter);
        EquipArmor(starter);
    }

    // Initializes the armor list with every tier
    void CreateArmors()
    {
        armors.Clear();
        armors.Add(new ArmorData("Bronze Robes", 1, 1, 0, 1.00f));
        armors.Add(new ArmorData("Iron Armor", 2, 5, 100, 1.10f));
        armors.Add(new ArmorData("Steel Armor", 3, 20, 1000, 1.25f));
        armors.Add(new ArmorData("Mithril Armor", 4, 40, 10000, 1.50f));
        armors.Add(new ArmorData("Rune Armor", 5, 60, 100000, 2.00f));
    }

    // Lookup helpers
    public ArmorData GetArmor(string name) => armors.Find(a => a.armorName == name);
    public List<ArmorData> GetAllArmors() => armors;
    public bool IsOwned(ArmorData armor) => armor != null && ownedArmors.Contains(armor.armorName);
    public ArmorData GetEquipped() => equippedArmor;

    // Highest unlock level requirement uses the higher of the two skills,
    // so progression in either skill can earn better armor.
    public bool CanBuy(ArmorData armor)
    {
        if (armor == null || IsOwned(armor)) return false;
        int playerLevel = Mathf.Max(
            LevelManager.Instance.woodcuttingLevel,
            LevelManager.Instance.miningLevel
        );
        if (playerLevel < armor.requiredLevel) return false;
        return GameManager.Instance.coins >= armor.cost;
    }

    // Attempts to purchase an armor. Returns true on success.
    public bool BuyArmor(ArmorData armor)
    {
        if (!CanBuy(armor)) return false;
        if (!GameManager.Instance.SpendCoins(armor.cost)) return false;
        ownedArmors.Add(armor.armorName);
        OnArmorChanged?.Invoke();
        return true;
    }

    public void EquipArmor(ArmorData armor)
    {
        if (!IsOwned(armor)) return;
        equippedArmor = armor;
        OnArmorChanged?.Invoke();
    }

    // Returns the XP multiplier from the equipped armor (defaults to 1.0 if none)
    public float GetXPMultiplier()
    {
        return equippedArmor != null ? equippedArmor.xpMultiplier : 1f;
    }
}