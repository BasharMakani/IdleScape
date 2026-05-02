using UnityEngine;

public class ResourceNodeVisual : MonoBehaviour
{
    
    [SerializeField] private GameObject[] woodPrefabs;   // Normal, Oak, Willow, Maple, Magic
    [SerializeField] private GameObject[] minePrefabs;   // Copper, Iron, Gold, Mithril, Runite
    [SerializeField] private Transform spawnPoint;       // spawnpoint for resource node models

    private GameObject currentModel;

    void Start()
    {
    ResourceNodeManager.Instance.OnNodeChanged += SwapModel;
    SwapModel(ResourceNodeManager.Instance.currentResource);
    }

    void OnDestroy()
    {
        ResourceNodeManager.Instance.OnNodeChanged -= SwapModel;
    }

   void SwapModel(ResourceData resource)
{
    if (currentModel != null)
        Destroy(currentModel);

    GameObject prefab = GetPrefabFor(resource);
    if (prefab == null)
    {
        return;
    }
    currentModel = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
}

    GameObject GetPrefabFor(ResourceData resource)
    {
        switch (resource.itemID)
        {
            case ItemID.NormalWood:  return woodPrefabs[0];
            case ItemID.OakWood:     return woodPrefabs[1];
            case ItemID.WillowWood:  return woodPrefabs[2];
            case ItemID.MapleWood:   return woodPrefabs[3];
            case ItemID.MagicWood:   return woodPrefabs[4];
            case ItemID.CopperOre:   return minePrefabs[0];
            case ItemID.IronOre:     return minePrefabs[1];
            case ItemID.GoldOre:     return minePrefabs[2];
            case ItemID.MithrilOre:  return minePrefabs[3];
            case ItemID.RuniteOre:   return minePrefabs[4];
            default: return null;
        }
    }
}