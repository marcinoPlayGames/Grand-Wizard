using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUpgradeUIController : MonoBehaviour
{
    public static WeaponUpgradeUIController Instance;

    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI nextValueText;
    public TextMeshProUGUI currentAbilityValueText;
    public TextMeshProUGUI nextAbilityValueText;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI coinsText;
    public TMP_Text damageText;
    public Button upgradeButton;

    // Podpięte z Inspectora
    public TMPro.TextMeshProUGUI basicDetailsText;
    public TMPro.TextMeshProUGUI abilityDetailsText;

    [Header("Additional Panels")]
    public GameObject mainPanel;

    private string currentWeaponId;

    private void Awake() => Instance = this;

    private void Start()
    {
        upgradePanel.SetActive(false);
        detailsPanel.SetActive(false);
    }

    public void ShowPanel(string weaponId)
    {
        currentWeaponId = weaponId;
        upgradePanel.SetActive(true);
        mainPanel.SetActive(false);
        detailsPanel.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        int maxLevel = WeaponUpgradeSystem.Instance.GetMaxLevel(currentWeaponId);

        int level = WeaponUpgradeSystem.Instance.GetLevel(currentWeaponId);
        float currentDamage = WeaponUpgradeSystem.Instance.GetTotalDamage(currentWeaponId, false);
        float nextDamage = WeaponUpgradeSystem.Instance.GetTotalDamage(currentWeaponId, true);
        float percent = currentDamage > 0 ? ((nextDamage - currentDamage) / currentDamage) * 100f : 0f;

        float currentAbilityDamage = currentDamage * 2;
        float nextAbilityDamage = nextDamage * 2;

        string currentLevelIcons = WeaponUpgradeSystem.Instance.GetModifiersIcons(currentWeaponId, false);
        string nextLevelIcons = WeaponUpgradeSystem.Instance.GetModifiersIcons(currentWeaponId, true);

        float atkMod = WeaponUpgradeSystem.Instance.GetModifier(currentWeaponId, "stat.Attack_Damage_modifier");
        float magMod = WeaponUpgradeSystem.Instance.GetModifier(currentWeaponId, "stat.Magic_Damage_modifier");

        int cost = WeaponUpgradeSystem.Instance.GetCost(currentWeaponId);
        int coins = GameManager.Instance.GetCoins();

        string currentDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(currentDamage, WeaponUITextFormatter.DamageType.Magic);
        string nextDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(nextDamage, WeaponUITextFormatter.DamageType.Magic);

        string currentAbilityDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(currentAbilityDamage, WeaponUITextFormatter.DamageType.Magic);
        string nextAbilityDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(nextAbilityDamage, WeaponUITextFormatter.DamageType.Magic);

        currentLevelText.text = $"Level {level}";

        if (level >= maxLevel)
        {
            nextLevelText.text = $"Max Level";
        }
        else
        {
            nextLevelText.text = $"Level {level + 1}";
        }

        currentValueText.text = $"{currentDamage_colored} ({currentLevelIcons})";
        nextValueText.text = $"{nextDamage_colored} ({nextLevelIcons})"; // <color=green>(+{percent:F1}%)</color>"

        currentAbilityValueText.text = $"{currentAbilityDamage_colored} ({currentLevelIcons})";
        nextAbilityValueText.text = $"{nextAbilityDamage_colored} ({nextLevelIcons})"; // <color=green>(+{percent:F1}%)</color>"

        costText.text = $"{cost}";
        coinsText.text = $"{coins}";

        SetWeaponDescription("LPM: Cast a fireball in front", "PPM: Cast a faster stronger fireball in front", currentDamage, currentLevelIcons, currentDamage * 2);

        // Możesz dodać osobne pola TMP_Text na modyfikatory:
        // attackModText.text = $"{atkMod}% Attack";
        // magicModText.text = $"{magMod}% Magic";
    }

    public void SetWeaponDescription(string basicAttackText, string abilityAttackText, float basicDamage, string statIcons, float abilityDamage)
    {
        float dmg = basicDamage;
        float bigDmg = abilityDamage;

        string additionalLabel = " [200%]";

        var damageString = WeaponUITextFormatter.GetColoredDamageText(basicAttackText, dmg, statIcons, "", WeaponUITextFormatter.DamageType.Magic);
        var bigDamageString = WeaponUITextFormatter.GetColoredDamageText(abilityAttackText, bigDmg, statIcons, additionalLabel, WeaponUITextFormatter.DamageType.Magic);

        damageText.text = $"{damageString}\n{bigDamageString}";
    }

    public void OnUpgradeClick()
    {
        bool success = WeaponUpgradeSystem.Instance.TryUpgrade(currentWeaponId);
        if (success)
            UpdateUI();
    }

    public void ClosePanel()
    {
        upgradePanel.SetActive(false);
    }

    [Header("UI Panels")]
    public GameObject upgradePanel;
    public GameObject detailsPanel;

    public void ShowDetails(string weaponId, bool isNextLevel)
    {
        currentWeaponId = weaponId;

        // Ukryj upgrade, pokaż details
        upgradePanel.SetActive(false);
        mainPanel.SetActive(false);
        detailsPanel.SetActive(true);

        // Zaktualizuj teksty w Details Panelu
        UpdateDetailsUI(weaponId, isNextLevel);
    }

    public void BackToUpgrade()
    {
        upgradePanel.SetActive(true);
        detailsPanel.SetActive(false);
        mainPanel.SetActive(false);
    }

    private void UpdateDetailsUI(string weaponId, bool isNextLevel)
    {
        var basicDetails = WeaponUpgradeSystem.Instance.GetWeaponBasicAttackDetails(weaponId, isNextLevel);
        var abilityDetails = WeaponUpgradeSystem.Instance.GetWeaponAbilityAttackDetails(weaponId, isNextLevel);

        // Załóżmy, że masz podpięte te pola tekstowe w inspectorze
        basicDetailsText.text = basicDetails;
        abilityDetailsText.text = abilityDetails;
    }

    public void BackToMainPanel()
    {
        detailsPanel.SetActive(false);
        upgradePanel.SetActive(false); // wyłącza cały WeaponUpgradeUI
        mainPanel.SetActive(true); // aktywuje panel z listą dostępnych broni
    }
}