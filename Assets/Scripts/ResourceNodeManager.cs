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

    // Prefabs for spawning bonus icons
    [SerializeField] private GameObject bonusIconPrefab;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-2f, 3f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(2f, 4.5f);

    [SerializeField] private float bonusSpawnCheckInterval = 1f;
    [SerializeField] private float bonusSpawnChance = 0.1f;

    private float bonusTimer;

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

    // Fired when tree spawns or is hit
    public event System.Action OnHealthChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateResources();
        LoadRandomUnlockedNode();

        bonusTimer = bonusSpawnCheckInterval;
    }

    void Update()
    {
        bonusTimer -= Time.deltaTime;

        if (bonusTimer <= 0f)
        {
            bonusTimer = bonusSpawnCheckInterval;

            if (Random.value <= bonusSpawnChance)
            {
                SpawnBonusIcon();
            }
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public void HealCurrentNode(int amount)
    {
        if (isFelling)
        {
            return;
        }

        currentHP += amount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        OnHealthChanged?.Invoke();
    }

    private TreeNode GetCurrentNodeVisual()
    {
        if (spawnPoint == null)
        {
            return null;
        }

        return spawnPoint.GetComponentInChildren<TreeNode>();
    }

    void SpawnBonusIcon()
    {
        Vector3 pos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            -69f
        );

        Instantiate(bonusIconPrefab, pos, Quaternion.identity);
    }

    public void MainClick()
    {
        Debug.Log("ResourceNodeManager: MainClick called.");

        if (isFelling)
        {
            Debug.Log("ResourceNodeManager: Click ignored because tree is felling.");
            return;
        }

        Debug.Log("ResourceNodeManager: Chop event invoked.");
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

        int damage = (int)GameManager.Instance.GetDamage();

        Debug.Log("ResourceNodeManager: Damage = " + damage);
        Debug.Log("ResourceNodeManager: HP before hit = " + currentHP);

        currentHP -= damage;

        Debug.Log("ResourceNodeManager: HP after hit = " + currentHP);

        OnTreeStruck?.Invoke();

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("ResourceNodeManager: HP reached 0. Starting FellSequence.");
            StartCoroutine(FellSequence());
        }

        OnHealthChanged?.Invoke();
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
        GameManager.Instance.AddCoins(currentResource.sellValue);

        int xp = Mathf.RoundToInt(currentResource.xpReward * GameManager.Instance.GetXPMultiplier());

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
        OnHealthChanged?.Invoke();
    }

    void CreateResources()
    {
        resources.Clear();

        resources.Add(new ResourceData(ItemID.OakWood, "Oak Tree", ActiveSkill.Woodcutting, 1, 1, 5, 15, 25));
        resources.Add(new ResourceData(ItemID.PineWood, "Pine Tree", ActiveSkill.Woodcutting, 2, 20, 15, 50, 100));
        resources.Add(new ResourceData(ItemID.ElmWood, "Elm Tree", ActiveSkill.Woodcutting, 3, 40, 40, 150, 500));
        resources.Add(new ResourceData(ItemID.AspenWood, "Aspen Tree", ActiveSkill.Woodcutting, 4, 60, 60, 1000, 1000));
    }
}