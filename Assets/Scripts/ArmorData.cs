[System.Serializable]
public class ArmorData
{
    // Stores Data for each armor piece. xpMultiplier is applied to XP gained on node completion (1.0 = no bonus)
    public string armorName;
    public int tier;
    public int requiredLevel;
    public long cost;
    public float xpMultiplier;

    public ArmorData(
        string armorName,
        int tier,
        int requiredLevel,
        long cost,
        float xpMultiplier
    )
    {
        this.armorName = armorName;
        this.tier = tier;
        this.requiredLevel = requiredLevel;
        this.cost = cost;
        this.xpMultiplier = xpMultiplier;
    }
}