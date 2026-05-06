using UnityEngine;

// Owns the chop animation entry point (Chop) and the impact-frame hook (RegisterAxeHit).
[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    // Trigger names defined in the Animator Controller.
    // Add a "Mine" trigger later if you make a mining animation.
    [SerializeField] private string chopTrigger = "Chop";

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Called by InputController when the screen is tapped.
    // Triggers the chop animation; the actual damage is dealt later from RegisterAxeHit().
    public void Chop()
    {
        // Currently only one animation; branch on currentSkill once mining has its own clip.
        animator.SetTrigger(chopTrigger);
    }

    // Hooked up as an Animation Event on the WoodChop clip at the impact frame.
    // Fires the actual hit so HP is subtracted when the axe visually connects.
    public void RegisterAxeHit()
    {
        if (ResourceNodeManager.Instance != null)
            ResourceNodeManager.Instance.MainClick();
    }
}