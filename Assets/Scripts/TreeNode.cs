using UnityEngine;

// Marks a GameObject as a hittable resource node and plays its fall animation.
public class TreeNode : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string fallTrigger = "Fall";

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning("TreeNode: No Animator found on " + gameObject.name);
        }
        else
        {
            Debug.Log("TreeNode: Animator found on " + gameObject.name);
        }
        
    if (animator != null)
        animator.ResetTrigger(fallTrigger);
    }

    public void PlayFall()
    {
        Debug.Log("TreeNode: PlayFall called on " + gameObject.name);

        if (animator != null)
        {
            Debug.Log("TreeNode: Setting fall trigger: " + fallTrigger);
            animator.SetTrigger(fallTrigger);
        }
        else
        {
            Debug.LogWarning("TreeNode: Cannot play fall animation because Animator is missing on " + gameObject.name);
        }
    }
}