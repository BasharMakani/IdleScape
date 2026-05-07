using UnityEngine;

public class ToolCollider : MonoBehaviour
{
    private Collider toolCollider;
    private bool hasHitThisSwing = false;

    void Awake()
    {
        toolCollider = GetComponent<Collider>();

        if (toolCollider != null)
        {
            toolCollider.isTrigger = true;
            toolCollider.enabled = false;
        }
    }

    public void EnableHitbox()
    {
        hasHitThisSwing = false;

        if (toolCollider != null)
        {
            toolCollider.enabled = true;
        }

        //Debug.Log("ToolCollider: Hitbox enabled.");
    }

    public void DisableHitbox()
    {
        if (toolCollider != null)
        {
            toolCollider.enabled = false;
        }

        //Debug.Log("ToolCollider: Hitbox disabled.");
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (toolCollider == null) return;
        if (!toolCollider.enabled) return;
        if (hasHitThisSwing) return;

        //Debug.Log("ToolCollider: Hit " + other.gameObject.name);

        TreeNode treeNode = other.GetComponentInParent<TreeNode>();

        if (treeNode == null)
        {
            Debug.Log("ToolCollider: Hit object is not a TreeNode.");
            return;
        }

        //Debug.Log("ToolCollider: TreeNode found. Applying hit.");

        hasHitThisSwing = true;

        if (ResourceNodeManager.Instance != null)
        {
            ResourceNodeManager.Instance.ApplyHit();
        }

        DisableHitbox();
    }
}