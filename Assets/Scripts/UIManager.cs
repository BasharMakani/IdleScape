using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject equipPanel;

    [SerializeField] Image currentSkillIcon;
    [SerializeField] Image otherSkillIcon;
    [SerializeField] Sprite woodcuttingSprite;
    [SerializeField] Sprite miningSprite;


    void Start()
    {
        ShowPanel(mainPanel);
        GameManager.Instance.OnSkillSwapped += UpdateSkillIcons;
        UpdateSkillIcons();
    }

    void ShowPanel(GameObject panel) {
        mainPanel.SetActive(false);
        shopPanel.SetActive(false);
        equipPanel.SetActive(false);
        panel.SetActive(true);
    }

    /* MAIN PAGE */
    public void OnShopButtonClicked(){
        ShowPanel(shopPanel);
    }

    public void OnEquipButtonClicked(){
        ShowPanel(equipPanel);
    }

    void UpdateSkillIcons()
    {
        ActiveSkill current = GameManager.Instance.currentSkill;
        currentSkillIcon.sprite = current == ActiveSkill.Woodcutting ? woodcuttingSprite : miningSprite;
        otherSkillIcon.sprite = current == ActiveSkill.Woodcutting ? miningSprite : woodcuttingSprite;
    }


    /* SHOP & EQUIP PAGE */
    public void OnBackButtonClicked(){
        ShowPanel(mainPanel);
    }

}
