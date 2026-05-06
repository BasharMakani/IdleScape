using UnityEngine;
using UnityEngine.EventSystems;

// Detects taps anywhere on the screen (excluding taps on UI elements like buttons),
// plays a tap sound, and asks the character to chop.
// Damage is dealt later from CharacterAnimator.RegisterAxeHit at the chop's impact frame.
[RequireComponent(typeof(AudioSource))]
public class InputController : MonoBehaviour
{
    [SerializeField] private CharacterAnimator character;
    [SerializeField] private AudioClip tapSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Skip taps that landed on a UI element so buttons keep working independently
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            HandleTap();
        }
    }

    void HandleTap()
    {
        if (tapSound != null) audioSource.PlayOneShot(tapSound);
        if (character != null) character.Chop();
    }
}