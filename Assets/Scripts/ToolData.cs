public enum ToolType { Axe, Pickaxe }

[System.Serializable]
public class ToolData
{
    // Stores Data for each tool, used for damage calculation, purchasing, and equipping
    public string toolName;
    public ToolType type;
    public int tier;
    public int requiredLevel;
    public long cost;
    public int damage;

    public ToolData(
        string toolName,
        ToolType type,
        int tier,
        int requiredLevel,
        long cost,
        int damage
    )
    {
        this.toolName = toolName;
        this.type = type;
        this.tier = tier;
        this.requiredLevel = requiredLevel;
        this.cost = cost;
        this.damage = damage;
    }
}