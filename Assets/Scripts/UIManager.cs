using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // Top Container Trackers (coins, xp)
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text woodcuttingLevelText;
    [SerializeField] private TMP_Text miningLevelText;
    [SerializeField] private Slider woodcuttingSlider;
    [SerializeField] private Slider miningSlider;

    // Script references
    [SerializeField] private GameManager gameManager;
    private LevelManager levelManager;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        ShowPanel(mainPanel);
        levelManager = gameManager.GetComponent<LevelManager>();
        

        // Initialize manager flags
        GameManager.Instance.OnSkillSwapped += UpdateSkillIcons;
        RefreshSkillIcons();

        GameManager.Instance.OnCoinsChanged += UpdateCoinDisplay;
        UpdateCoinDisplay();

        LevelManager.Instance.OnXPChanged += UpdateXPDisplay;
        UpdateXPDisplay();
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

    // Called from GameManager when skill swap button pressed
    void UpdateSkillIcons()
    {
        levelManager.AddWoodcuttingXP(8); // DELETE ME ONCE IMPLEMENTED
        levelManager.AddMiningXP(5); // DELETE ME ONCE IMPLEMENTED
        gameManager.AddCoins(100); // DELETE ME ONCE IMPLEMENTED
        
        audioSource.PlayOneShot(clickConfirm);
        RefreshSkillIcons();
    }

    void RefreshSkillIcons()
    {
        ActiveSkill current = GameManager.Instance.currentSkill;
        currentSkillIcon.sprite = current == ActiveSkill.Woodcutting ? woodcuttingSprite : miningSprite;
        otherSkillIcon.sprite = current == ActiveSkill.Woodcutting ? miningSprite : woodcuttingSprite;
    }

    // Called from GameManager when coin count increments
    void UpdateCoinDisplay()
    {
        long coins = GameManager.Instance.coins;
        string coinDisplay;

        if      (coins < 0)                 coinDisplay = "INF";
        else if (coins >= 1000000000000000) coinDisplay = "INF";
        else if (coins >= 1000000000000)    coinDisplay = FormatCoins(coins, 1000000000000f, "T");
        else if (coins >= 1000000000)       coinDisplay = FormatCoins(coins, 1000000000f, "B");
        else if (coins >= 1000000)          coinDisplay = FormatCoins(coins, 1000000f, "M");
        else if (coins >= 1000)             coinDisplay = FormatCoins(coins, 1000f, "K");
        else                                coinDisplay = coins.ToString();
        
        coinText.text = coinDisplay;
    }

    // Helper method for extracting exactly 3 digits for coin display
    string FormatCoins(long coins, float divisor, string suffix)
    {
        float value = coins / divisor;
        string format = value >= 100 ? "0" : value >= 10 ? "0.#" : "0.##";
        return value.ToString(format) + suffix;
    }

    // Called from LevelManager, updates both wood and ore XP bars/counter
    void UpdateXPDisplay()
    {
        woodcuttingLevelText.text = "LVL " + levelManager.woodcuttingLevel;
        miningLevelText.text = "LVL " + levelManager.miningLevel;

        woodcuttingSlider.value = (float)levelManager.woodcuttingXP / levelManager.GetXPToNextLevel(levelManager.woodcuttingLevel);
        miningSlider.value = (float)levelManager.miningXP / levelManager.GetXPToNextLevel(levelManager.miningLevel);
    }


    /* SHOP & EQUIP PAGE */
    public void OnBackButtonClicked(){
        audioSource.PlayOneShot(inventory2);
        ShowPanel(mainPanel);
    }

}
