using UnityEngine;

public class ResourceNodeVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] woodPrefabs;
    [SerializeField] private Transform spawnPoint;

    private GameObject currentModel;

    void Start()
    {
        ResourceNodeManager.Instance.OnNodeChanged += SwapModel;
        SwapModel(ResourceNodeManager.Instance.currentResource);
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
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        GameObject prefab = GetPrefabFor(resource);

        if (prefab == null)
        {
            return;
        }

        currentModel = Instantiate(prefab, spawnPoint);

        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        currentModel.transform.localScale = new Vector3(100f, 100f, 100f);
    }

    GameObject GetPrefabFor(ResourceData resource)
    {
        switch (resource.itemID)
        {
            case ItemID.NormalWood:
                return woodPrefabs[0];

            case ItemID.OakWood:
                return woodPrefabs[1];

            case ItemID.WillowWood:
                return woodPrefabs[2];

            case ItemID.MapleWood:
                return woodPrefabs[3];

            case ItemID.MagicWood:
                return woodPrefabs[4];

            default:
                return null;
        }
    }
}