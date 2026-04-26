using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Primary Panels
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject equipPanel;

    // Swap-Skill Button Icon Fields
    [SerializeField] Image currentSkillIcon;
    [SerializeField] Image otherSkillIcon;
    [SerializeField] Sprite woodcuttingSprite;
    [SerializeField] Sprite miningSprite;

    // Audio
    private AudioSource audioSource;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip clickConfirm;
    [SerializeField] private AudioClip inventory1;
    [SerializeField] private AudioClip inventory2;


    void Start()
    {
        ShowPanel(mainPanel);
        audioSource = GetComponent<AudioSource>();

        GameManager.Instance.OnSkillSwapped += UpdateSkillIcons;
        RefreshSkillIcons();
    }

    void ShowPanel(GameObject panel) {
        mainPanel.SetActive(false);
        shopPanel.SetActive(false);
        equipPanel.SetActive(false);
        panel.SetActive(true);
    }

    /* MAIN PAGE */
    public void OnShopButtonClicked(){
        audioSource.PlayOneShot(inventory1);
        ShowPanel(shopPanel);
    }

    public void OnEquipButtonClicked(){
        audioSource.PlayOneShot(inventory1);
        ShowPanel(equipPanel);
    }

    // Called from GameManager listener, effectively OnSwapSkillButtonClicked()
    void UpdateSkillIcons()
    {
        audioSource.PlayOneShot(clickConfirm);
        RefreshSkillIcons();
    }

    void RefreshSkillIcons()
    {
        ActiveSkill current = GameManager.Instance.currentSkill;
        currentSkillIcon.sprite = current == ActiveSkill.Woodcutting ? woodcuttingSprite : miningSprite;
        otherSkillIcon.sprite = current == ActiveSkill.Woodcutting ? miningSprite : woodcuttingSprite;
    }


    /* SHOP & EQUIP PAGE */
    public void OnBackButtonClicked(){
        audioSource.PlayOneShot(inventory2);
        ShowPanel(mainPanel);
    }

}
