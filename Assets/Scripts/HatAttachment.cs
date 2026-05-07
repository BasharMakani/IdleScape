using UnityEngine;

public class HatAttachment : MonoBehaviour
{
    [SerializeField] private Transform headBone;
    private GameObject currentHat;

    void Start()
    {
        GameManager.Instance.OnVanityChanged += EquipHat;
    }

    void Destroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnVanityChanged -= EquipHat;
    }

    void EquipHat(int vanityId)
    {
        if (currentHat != null)
            Destroy(currentHat);

        VanityOption option = System.Array.Find(
            GameManager.Instance.vanityOptions, o => o.id == vanityId);

        if (option == null || option.prefab == null)
            return;

        currentHat = Instantiate(option.prefab);
        currentHat.transform.SetParent(headBone, true);
        currentHat.transform.localPosition = option.positionOffset;
        currentHat.transform.localRotation = Quaternion.Euler(option.rotationOffset);
    }
}
