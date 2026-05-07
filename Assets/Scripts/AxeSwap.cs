using UnityEngine;

public class AxeSwap : MonoBehaviour
{
    [SerializeField] private GameObject defaultAxePrefab;
    [SerializeField] private GameObject goldenAxePrefab;
    [SerializeField] private Transform axeBone;
    [SerializeField] private CharacterAnimator characterAnimator;
    [SerializeField] private int upgradeThreshold;
    [SerializeField] private Vector3 axeOffset;

    private GameObject currentAxe;

    void Start()
    {
        SpawnAxe(defaultAxePrefab, axeOffset);
        GameManager.Instance.OnUpgradePurchased += CheckSwap;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnUpgradePurchased -= CheckSwap;
    }

    void CheckSwap()
    {
        if (GameManager.Instance.toolUpgradeCount >= upgradeThreshold)
        {
            SpawnAxe(goldenAxePrefab, axeOffset);
        }
    }

    void SpawnAxe(GameObject prefab, Vector3 axeOffset)
    {
        if (currentAxe != null)
            Destroy(currentAxe);

        currentAxe = Instantiate(prefab);
        currentAxe.transform.SetParent(axeBone, true);
        currentAxe.transform.localPosition = axeOffset;
        characterAnimator.toolCollider = currentAxe.GetComponent<ToolCollider>();
    }
}
