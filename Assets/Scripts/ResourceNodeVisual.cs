using UnityEngine;

public class ResourceNodeVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] woodPrefabs;  // Normal, Oak, Willow, Maple, Magic    
    [SerializeField] private Transform spawnPoint;

    private GameObject currentModel;

    void Start()
    {
        ResourceNodeManager.Instance.OnNodeChanged += SwapModel;
        if (ResourceNodeManager.Instance.currentResource != null)
            SwapModel(ResourceNodeManager.Instance.currentResource);
    }

    void OnDestroy()
    {
        if (ResourceNodeManager.Instance != null)
            ResourceNodeManager.Instance.OnNodeChanged -= SwapModel;
    }

    void SwapModel(ResourceData resource)
{
    if (resource == null) return;

    if (currentModel != null)
        Destroy(currentModel);

    GameObject prefab = GetPrefabFor(resource);
    if (prefab == null) return;

    currentModel = Instantiate(prefab, spawnPoint);
    currentModel.transform.localPosition = Vector3.zero;
    currentModel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    currentModel.transform.localScale = new Vector3(1f, 1f, 1f);

    Animator anim = currentModel.GetComponentInChildren<Animator>();
    if (anim != null)
    {
        int treeType = resource.itemID switch
        {
            ItemID.NormalWood  => 2, // Oak
            ItemID.OakWood     => 1, // Pine
            ItemID.WillowWood  => 0, // Elm
            ItemID.MapleWood   => 3, // Aspen
            _                  => 0
        };
        anim.SetInteger("TreeType", treeType);
    }
}

    GameObject GetPrefabFor(ResourceData resource)
    {
        switch (resource.itemID)
        {
            case ItemID.NormalWood:  return woodPrefabs != null && woodPrefabs.Length > 0 ? woodPrefabs[0] : null;
            case ItemID.OakWood:     return woodPrefabs != null && woodPrefabs.Length > 1 ? woodPrefabs[1] : null;
            case ItemID.WillowWood:  return woodPrefabs != null && woodPrefabs.Length > 2 ? woodPrefabs[2] : null;
            case ItemID.MapleWood:   return woodPrefabs != null && woodPrefabs.Length > 3 ? woodPrefabs[3] : null;
            case ItemID.MagicWood:   return woodPrefabs != null && woodPrefabs.Length > 4 ? woodPrefabs[4] : null;

            default: return null;
        }
    }
}