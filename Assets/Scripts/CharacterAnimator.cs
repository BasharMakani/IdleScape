using UnityEngine;
using UnityEngine.EventSystems;

// Detects taps anywhere on the screen, plays a tap sound,
// triggers the character's chop animation, and controls the axe hitbox.
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private string chopTrigger = "Chop";

    [Header("Cooldown")]
    [SerializeField, Range(0.1f, 5f)] private float chopCooldown = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip tapSound;

    public ToolCollider toolCollider;
    private Animator animator;
    private AudioSource audioSource;

    private float nextChopTime = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (toolCollider != null)
        {
            toolCollider.DisableHitbox();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Chop();
        }
    }

    public void Chop()
    {
        if (Time.time < nextChopTime)
        {
            return;
        }

        nextChopTime = Time.time + chopCooldown;

        if (tapSound != null)
        {
            audioSource.PlayOneShot(tapSound);
        }

        animator.SetTrigger(chopTrigger);
    }

    public void EnableAxeHitbox()
    {
        if (toolCollider != null)
        {
            toolCollider.EnableHitbox();
        }
    }

    public void DisableAxeHitbox()
    {
        if (toolCollider != null)
        {
            toolCollider.DisableHitbox();
        }
    }

    public void RegisterAxeHit()
    {
        EnableAxeHitbox();
    }
}