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

    [Header("Swap Skill Assets")]
    [SerializeField] Image currentSkillIcon;
    [SerializeField] Image otherSkillIcon;
    [SerializeField] Sprite woodcuttingSprite;
    [SerializeField] Sprite miningSprite;

    [Header("Audio Files")] 
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip clickConfirm;
    [SerializeField] private AudioClip inventory1;
    [SerializeField] private AudioClip inventory2;
    private AudioSource audioSource;

    [Header("Top Container Fields")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text woodcuttingLevelText;
    [SerializeField] private TMP_Text miningLevelText;
    [SerializeField] private Slider woodcuttingSlider;
    [SerializeField] private Slider miningSlider;

    [Header("Shop")]
    [SerializeField] private TMP_Text shopCoinText;

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
        unlockedOptions = System.Array.FindAll(GameManager.Instance.vanityOptions, o => o.isPurchased);
        vanityIndex = 0;
        RefreshVanityDisplay();
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
        shopCoinText.text = coinDisplay;
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

    /* EQUIP PAGE */

    // Changes current gender and sets check mark position to corresponding image, invokes genderChanged?
    public void OnManButtonClicked(){
        if (currentGender != Gender.Man) audioSource.PlayOneShot(click);
        currentGender = Gender.Man;
        RectTransform rt = selectedGenderSymbol.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(SelectSymbolManX, rt.anchoredPosition.y);
        OnGenderChanged?.Invoke();
    }
    public void OnWomanButtonClicked(){
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
