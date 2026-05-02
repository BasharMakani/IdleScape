using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ResourceNodeManager : MonoBehaviour
{
    public static ResourceNodeManager Instance { get; private set; }
    void Awake() { Instance = this; }
    public event System.Action<ResourceData> OnNodeChanged;
    ///Manages Current Node Health
    public int currentHP;
    public int maxHP;
    //Manages Current active Resource
    public ResourceData currentResource;
    //List of all resources, used for determining what node to spawn on skill swap and on node completion
    private List<ResourceData> resources = new List<ResourceData>();

    void Start()
    {
        //Initialize Resource List and Load Starting Node
        CreateResources();
        //Load Starting Node
        LoadRandomUnlockedNode();
        //reloads nodes when swapping skills
        GameManager.Instance.OnSkillSwapped += LoadRandomUnlockedNode;
    }
    //Called directly from click, reduces node HP by the equipped tool's damage and checks for completion
    public void MainClick()
    {
        int damage = ToolManager.Instance.GetCurrentDamage(GameManager.Instance.currentSkill);
        currentHP -= damage;
        if (currentHP <= 0)
        {
            CompleteNode();
        }
    }
    //Handles Node Completion, gives rewards and loads new node
    void CompleteNode()
    {
        //Give item + coin rewards
        GameManager.Instance.AddItem(currentResource.itemID, 1);
        GameManager.Instance.AddCoins(currentResource.sellValue);
        //Apply armor XP multiplier, then add XP based on node type
        int xp = Mathf.RoundToInt(currentResource.xpReward * ArmorManager.Instance.GetXPMultiplier());
        if (currentResource.skill == ActiveSkill.Woodcutting)
        {
            LevelManager.Instance.AddWoodcuttingXP(xp);
        }
        else
        {
            LevelManager.Instance.AddMiningXP(xp);
        }
        LoadRandomUnlockedNode();
    }
    //Loads a random node from the list of resources that the player is currently high enough level to access, based on their current active skill
    void LoadRandomUnlockedNode()
    {
        ActiveSkill skill = GameManager.Instance.currentSkill;
        //Gets lvl based on skill
        int level = LevelManager.Instance.GetLevel(skill);
        //Filters resource list to only include nodes matching current skill and player level, then selects random node from that filtered list
        List<ResourceData> unlockedResources = resources
            .Where(r => r.skill == skill && level >= r.requiredLevel)
            .ToList();
        currentResource = unlockedResources[Random.Range(0, unlockedResources.Count)];
        maxHP = currentResource.maxHP;
        currentHP = maxHP;
        OnNodeChanged?.Invoke(currentResource);
    }
    //Initializes the resource list with all nodes and their associated data
    void CreateResources()
    {
        resources.Clear();
        resources.Add(new ResourceData(ItemID.NormalWood, "Normal Tree", ActiveSkill.Woodcutting, 1, 1, 5, 5, 1));
        resources.Add(new ResourceData(ItemID.OakWood, "Oak Tree", ActiveSkill.Woodcutting, 2, 20, 15, 20, 5));
        resources.Add(new ResourceData(ItemID.WillowWood, "Willow Tree", ActiveSkill.Woodcutting, 3, 40, 35, 60, 15));
        resources.Add(new ResourceData(ItemID.MapleWood, "Maple Tree", ActiveSkill.Woodcutting, 4, 60, 75, 150, 40));
        resources.Add(new ResourceData(ItemID.MagicWood, "Magic Tree", ActiveSkill.Woodcutting, 5, 80, 150, 400, 100));
        resources.Add(new ResourceData(ItemID.CopperOre, "Copper Rock", ActiveSkill.Mining, 1, 1, 5, 5, 1));
        resources.Add(new ResourceData(ItemID.IronOre, "Iron Rock", ActiveSkill.Mining, 2, 20, 15, 20, 5));
        resources.Add(new ResourceData(ItemID.GoldOre, "Gold Rock", ActiveSkill.Mining, 3, 40, 35, 60, 15));
        resources.Add(new ResourceData(ItemID.MithrilOre, "Mithril Rock", ActiveSkill.Mining, 4, 60, 75, 150, 40));
        resources.Add(new ResourceData(ItemID.RuniteOre, "Runite Rock", ActiveSkill.Mining, 5, 80, 150, 400, 100));
    }
}