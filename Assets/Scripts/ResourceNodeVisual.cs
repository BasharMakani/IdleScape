using UnityEngine;

public class ResourceNodeVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] woodPrefabs;  // Oak, Pine, Elm, Aspen
    [SerializeField] private Transform spawnPoint;

    private GameObject currentModel;

    void Start()
    {
        ResourceNodeManager.Instance.OnNodeChanged += SwapModel;

        if (ResourceNodeManager.Instance.currentResource != null)
        {
            SwapModel(ResourceNodeManager.Instance.currentResource);
        }
    }

    void OnDestroy()
    {
        if (ResourceNodeManager.Instance != null)
        {
            ResourceNodeManager.Instance.OnNodeChanged -= SwapModel;
        }
    }

    void SwapModel(ResourceData resource)
    {
        if (resource == null) return;

        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        GameObject prefab = GetPrefabFor(resource);

        if (prefab == null)
        {
            Debug.LogWarning("ResourceNodeVisual: No prefab found for " + resource.itemID);
            return;
        }

        currentModel = Instantiate(prefab);
        currentModel.transform.SetParent(spawnPoint, false);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;
        currentModel.transform.localScale = Vector3.one;

        //Debug.Log("ResourceNodeVisual: Spawned new tree model: " + currentModel.name);
        //Debug.Log("ResourceNodeVisual: Spawn point position: " + spawnPoint.position);
        //Debug.Log("ResourceNodeVisual: Current model position: " + currentModel.transform.position);

        TreeNode treeNode = currentModel.GetComponentInChildren<TreeNode>();
        Collider treeCollider = currentModel.GetComponentInChildren<Collider>();

        //Debug.Log("ResourceNodeVisual: Spawned tree has TreeNode: " + (treeNode != null));
        //Debug.Log("ResourceNodeVisual: Spawned tree has Collider: " + (treeCollider != null));

        //if (treeCollider != null)
        //{
        //    Debug.Log("ResourceNodeVisual: Tree collider enabled: " + treeCollider.enabled);
        //    Debug.Log("ResourceNodeVisual: Tree collider is trigger: " + treeCollider.isTrigger);
        //    Debug.Log("ResourceNodeVisual: Tree collider bounds: " + treeCollider.bounds);
        //}

        Animator anim = currentModel.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            int treeType = resource.itemID switch
            {
                ItemID.OakWood => 2,   // Oak
                ItemID.PineWood => 1,  // Pine
                ItemID.ElmWood => 0,   // Elm
                ItemID.AspenWood => 3, // Aspen
                _ => 0
            };

            anim.SetInteger("TreeType", treeType);
        }
        else
        {
            Debug.LogWarning("ResourceNodeVisual: No Animator found on spawned tree.");
        }
    }

    GameObject GetPrefabFor(ResourceData resource)
    {
        switch (resource.itemID)
        {
            case ItemID.OakWood:
                return woodPrefabs != null && woodPrefabs.Length > 0 ? woodPrefabs[0] : null;

            case ItemID.PineWood:
                return woodPrefabs != null && woodPrefabs.Length > 1 ? woodPrefabs[1] : null;

            case ItemID.ElmWood:
                return woodPrefabs != null && woodPrefabs.Length > 2 ? woodPrefabs[2] : null;

            case ItemID.AspenWood:
                return woodPrefabs != null && woodPrefabs.Length > 3 ? woodPrefabs[3] : null;

            default:
                return null;
        }
    }
}