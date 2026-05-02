using UnityEngine;
[System.Serializable]
public class ResourceData
{
    //Stores Data for each resource node, used for determining what node to spawn and what rewards to give on completion
    public ItemID itemID;
    public string resourceName;
    public ActiveSkill skill;
    public int tier;
    public int requiredLevel;
    public int maxHP;
    public int xpReward;
    public int sellValue;
    public GameObject prefab;

    public ResourceData(
        ItemID itemID,
        string resourceName,
        ActiveSkill skill,
        int tier,
        int requiredLevel,
        int maxHP,
        int xpReward,
        int sellValue
    )
    {
        this.itemID = itemID;
        this.resourceName = resourceName;
        this.skill = skill;
        this.tier = tier;
        this.requiredLevel = requiredLevel;
        this.maxHP = maxHP;
        this.xpReward = xpReward;
        this.sellValue = sellValue;
    }
}