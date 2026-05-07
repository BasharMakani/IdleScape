using UnityEngine;

public class ToolCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("ToolCollider: Hit " + other.gameObject.name);

        TreeNode treeNode = other.GetComponentInParent<TreeNode>();

        if (treeNode != null)
        {
            Debug.Log("ToolCollider: TreeNode found. Applying hit.");
            ResourceNodeManager.Instance.ApplyHit();
        }
        else
        {
            Debug.Log("ToolCollider: Hit object is not a TreeNode.");
        }
    }
}