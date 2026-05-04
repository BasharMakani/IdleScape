using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    // Per-skill XP and level state
    public int woodcuttingLevel = 1;
    public int miningLevel = 1;
    public int woodcuttingXP = 0;
    public int miningXP = 0;

    public int xpToLevelMult = 50;

    public event System.Action OnXPChanged;

    void Awake()
    {
        Instance = this;
    }

    // Adds xp to woodcutting, handles level up logic
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

    // Adds xp to mining, handles level up logic
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

    // Convenience accessor used by tool/armor managers to gate purchases
    public int GetLevel(ActiveSkill skill)
    {
        return skill == ActiveSkill.Woodcutting ? woodcuttingLevel : miningLevel;
    }

    // XP required to reach next level
    public int GetXPToNextLevel(int level)
    {
        return level * xpToLevelMult;
    }
}
