using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum Gender { Man, Woman }

public class UIManager : MonoBehaviour
{
    [Header("Primary Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject equipPanel;

    [Header("Audio Files")] 
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip clickConfirm;
    [SerializeField] private AudioClip inventory1;
    [SerializeField] private AudioClip inventory2;
    private AudioSource audioSource;

    [Header("Top Container Fields")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text woodcuttingLevelText;
    [SerializeField] private Slider woodcuttingSlider;

    [Header("Shop")]
    [SerializeField] private TMP_Text shopCoinText;
    [SerializeField] private TMP_Text toolUpgradePrice;
    [SerializeField] private TMP_Text chestUpgradePrice;
    [SerializeField] private TMP_Text legUpgradePrice;

    [Header("Gender")]
    [SerializeField] private GameObject selectedGenderSymbol;
    public Gender currentGender;
    private const float SelectSymbolManX = -100f;
    private const float SelectSymbolWomanX = 500f;
    public event Action OnGenderChanged;
    
    
    [Header("Vanity")]
    [SerializeField] private Image vanityImage;
    [SerializeField] private TMP_Text vanityNameText;
    private VanityOption[] unlockedOptions;
    private int vanityIndex = 0;
    


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

        currentGender = Gender.Man; // Default is man
        vanityIndex = 0; // Default is no vanity

        // Initialize manager flags

        GameManager.Instance.OnCoinsChanged += UpdateCoinDisplay;
        UpdateCoinDisplay();

        GameManager.Instance.OnUpgradePurchased += UpdateShopDisplay;
        UpdateShopDisplay();

        LevelManager.Instance.OnXPChanged += UpdateXPDisplay;
        UpdateXPDisplay();
    }

    void ShowPanel(GameObject panel) {
        mainPanel.SetActive(false);
        shopPanel.SetActive(false);
        equipPanel.SetActive(false);
        panel.SetActive(true);
    }

    /*-------- MAIN PAGE ----------*/
    public void OnShopButtonClicked(){
        audioSource.PlayOneShot(inventory1);
        UpdateShopDisplay();
        ShowPanel(shopPanel);
    }

    public void OnEquipButtonClicked(){
        audioSource.PlayOneShot(inventory1);
        unlockedOptions = System.Array.FindAll(GameManager.Instance.vanityOptions, o => o.isPurchased);
        RefreshVanityDisplay();
        ShowPanel(equipPanel);
    }

    // Called from GameManager when coin count increments
    void UpdateCoinDisplay()
    {
        string coinDisplay = FormatValue(GameManager.Instance.coins);        
        coinText.text = coinDisplay;
        shopCoinText.text = coinDisplay;
    }

    // Helper method for adding suffixs and truncating coin count
    string FormatValue(long value)
    {
        if      (value < 0)                 return "INF";
        else if (value >= 1000000000000000) return "INF";
        else if (value >= 1000000000000)    return FormatCoins(value, 1000000000000f, "T");
        else if (value >= 1000000000)       return FormatCoins(value, 1000000000f, "B");
        else if (value >= 1000000)          return FormatCoins(value, 1000000f, "M");
        else if (value >= 1000)             return FormatCoins(value, 1000f, "K");
        else                                return value.ToString();
    }

    // Helper method for extracting exactly 3 digits for coin display
    string FormatCoins(long coins, float divisor, string suffix)
    {
        float value = coins / divisor;
        string format = value >= 100 ? "0" : value >= 10 ? "0.#" : "0.##";
        return value.ToString(format) + suffix;
    }

    // Called from LevelManager, updates both wood and ore XP bars/counter
    void UpdateXPDisplay(){
        woodcuttingLevelText.text = "LVL " + levelManager.woodcuttingLevel;
        woodcuttingSlider.value = (float)levelManager.woodcuttingXP / levelManager.GetXPToNextLevel(levelManager.woodcuttingLevel);
    }


    /*--------- SHOP & EQUIP PAGE -----------*/
    public void OnBackButtonClicked(){
        audioSource.PlayOneShot(inventory2);
        ShowPanel(mainPanel);
    }

    /*--------- SHOP PAGE --------*/

    // 0 is tool,        1 is chestplate, 2 is leggings
    // 0 is swing speed, 1 is xp yield,   2 is coin yield
    public void OnShopUpgradeClicked(int upgradeId)
    {
        bool success = true;
        Upgrade upgrade = GameManager.Instance.upgrades[upgradeId];

        if (GameManager.Instance.coins < upgrade.cost) success = false;
        if (LevelManager.Instance.GetLevel(ActiveSkill.Woodcutting) < upgrade.requiredLevel) success = false;

        if (success){
            audioSource.PlayOneShot(clickConfirm);
            GameManager.Instance.PurchaseUpgrade(upgradeId);
        } else {
            audioSource.PlayOneShot(click);
        }
    }

    // Update relavent font text and color for each button in the shop
    public void UpdateShopDisplay()
    {
        Upgrade[] upgrades = GameManager.Instance.upgrades;
        int currentLevel = LevelManager.Instance.GetLevel(ActiveSkill.Woodcutting);
        ColorUtility.TryParseHtmlString("#E9D14C", out Color gold);

        // Tool Button, price if above current level cap, level (in red) if below
        if (upgrades[0].requiredLevel <= currentLevel){
            toolUpgradePrice.text = "$ " + FormatValue(upgrades[0].cost);
            toolUpgradePrice.color = gold;
        } else {
            toolUpgradePrice.text = "LVL " + upgrades[0].requiredLevel;
            toolUpgradePrice.color = Color.red;
        }
        
        // Chestplate Button
        if (upgrades[1].requiredLevel <= currentLevel){
            chestUpgradePrice.text = "$ " + FormatValue(upgrades[1].cost);
            chestUpgradePrice.color = gold;
        } else {
            chestUpgradePrice.text = "LVL " + upgrades[1].requiredLevel;
            chestUpgradePrice.color = Color.red;
        }

        // Leggings Button
        if (upgrades[2].requiredLevel <= currentLevel){
            legUpgradePrice.text = "$ " + FormatValue(upgrades[2].cost);
            legUpgradePrice.color = gold;
        } else {
            legUpgradePrice.text = "LVL " + upgrades[2].requiredLevel;
            legUpgradePrice.color = Color.red;
        }
    }


    /*---------- EQUIP PAGE ----------*/
    // Changes current gender and sets check mark position to corresponding image, invokes genderChanged?
    public void OnManButtonClicked()
    {
        if (currentGender != Gender.Man) audioSource.PlayOneShot(click);
        currentGender = Gender.Man;

        RectTransform rt = selectedGenderSymbol.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(SelectSymbolManX, rt.anchoredPosition.y);

        OnGenderChanged?.Invoke();
    }
    public void OnWomanButtonClicked()
    {
        if (currentGender != Gender.Woman) audioSource.PlayOneShot(click);
        currentGender = Gender.Woman;

        RectTransform rt = selectedGenderSymbol.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(SelectSymbolWomanX, rt.anchoredPosition.y);

        OnGenderChanged?.Invoke();
    }


    // Vanity Control Methods
    public void OnVanityLeftClicked() {
        audioSource.PlayOneShot(click);
        vanityIndex = (vanityIndex - 1 + unlockedOptions.Length) % unlockedOptions.Length;
        RefreshVanityDisplay();
    }

    public void OnVanityRightClicked() {
        audioSource.PlayOneShot(click);
        vanityIndex = (vanityIndex + 1) % unlockedOptions.Length;
        RefreshVanityDisplay();
    }

    void RefreshVanityDisplay() {
        VanityOption option = unlockedOptions[vanityIndex];
        vanityNameText.text = option.name;
        vanityImage.sprite = option.vanitySprite;
        GameManager.Instance.SetVanity(option.id);
    }






}
