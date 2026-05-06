using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ResourceNodeManager : MonoBehaviour
{
    public static ResourceNodeManager Instance { get; private set; }

    // Manages current node health
    public int currentHP;
    public int maxHP;

    // Manages current active resource
    public ResourceData currentResource { get; private set; }

    // Tells ResourceNodeVisual when the node changes
    public event System.Action<ResourceData> OnNodeChanged;

    // List of all resources, used for determining what node to spawn
    private List<ResourceData> resources = new List<ResourceData>();

    // Drag your SpawnPoint Transform here
    [SerializeField] private Transform spawnPoint;

    // Seconds to wait after triggering the fall animation before respawning the next node
    [SerializeField] private float fellDuration = 1.5f;

    // True while the felling sequence is playing
    private bool isFelling = false;

    // Fired when the player taps to swing
    public event System.Action OnChopStarted;

    // Fired when the axe blade actually contacts the node
    public event System.Action OnTreeStruck;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateResources();
        LoadRandomUnlockedNode();

        GameManager.Instance.OnSkillSwapped += LoadRandomUnlockedNode;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSkillSwapped -= LoadRandomUnlockedNode;
        }
    }

    private TreeNode GetCurrentNodeVisual()
    {
        if (spawnPoint == null)
        {
            return null;
        }

        return spawnPoint.GetComponentInChildren<TreeNode>();
    }

    public void MainClick()
    {
        if (isFelling) return;

        OnChopStarted?.Invoke();
    }

    public void ApplyHit()
    {
        Debug.Log("ResourceNodeManager: ApplyHit called.");

        if (isFelling)
        {
            Debug.Log("ResourceNodeManager: Hit ignored because tree is already felling.");
            return;
        }

        int damage = ToolManager.Instance.GetCurrentDamage(GameManager.Instance.currentSkill);

        Debug.Log("ResourceNodeManager: Damage = " + damage);
        Debug.Log("ResourceNodeManager: HP before hit = " + currentHP);

        currentHP -= damage;

        Debug.Log("ResourceNodeManager: HP after hit = " + currentHP);

        OnTreeStruck?.Invoke();

        if (currentHP <= 0)
        {
            Debug.Log("ResourceNodeManager: HP reached 0. Starting FellSequence.");
            StartCoroutine(FellSequence());
        }
    }

    IEnumerator FellSequence()
    {
        Debug.Log("ResourceNodeManager: FellSequence started.");

        isFelling = true;

        TreeNode node = GetCurrentNodeVisual();

        if (node != null)
        {
            Debug.Log("ResourceNodeManager: TreeNode found. Calling PlayFall.");
            node.PlayFall();
        }
        else
        {
            Debug.LogWarning("ResourceNodeManager: No TreeNode found under spawnPoint.");
        }

        yield return new WaitForSeconds(fellDuration);

        Debug.Log("ResourceNodeManager: Fell wait finished. Completing node.");

        CompleteNode();

        isFelling = false;
    }

    void CompleteNode()
    {
        GameManager.Instance.AddItem(currentResource.itemID, 1);
        GameManager.Instance.AddCoins(currentResource.sellValue);

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

    void LoadRandomUnlockedNode()
    {
        ActiveSkill skill = GameManager.Instance.currentSkill;

        int level = LevelManager.Instance.GetLevel(skill);

        List<ResourceData> unlockedResources = resources
            .Where(r => r.skill == skill && level >= r.requiredLevel)
            .ToList();

        currentResource = unlockedResources[Random.Range(0, unlockedResources.Count)];

        maxHP = currentResource.maxHP;
        currentHP = maxHP;

        OnNodeChanged?.Invoke(currentResource);
    }

    void CreateResources()
    {
        resources.Clear();

        resources.Add(new ResourceData(ItemID.NormalWood, "Oak Tree", ActiveSkill.Woodcutting, 1, 1, 5, 5, 1));
        resources.Add(new ResourceData(ItemID.OakWood, "Pine Tree", ActiveSkill.Woodcutting, 2, 20, 15, 20, 5));
        resources.Add(new ResourceData(ItemID.WillowWood, "Elm Tree", ActiveSkill.Woodcutting, 3, 40, 35, 60, 15));
        resources.Add(new ResourceData(ItemID.MapleWood, "Aspen Tree", ActiveSkill.Woodcutting, 4, 60, 75, 150, 40));
    }
}