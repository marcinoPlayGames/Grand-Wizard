using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUpgradeUIController : MonoBehaviour
{
    public static WeaponUpgradeUIController Instance;

    public GameObject panel;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI nextValueText;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI coinsText;
    public Button upgradeButton;

    private string currentWeaponId;

    private void Awake() => Instance = this;

    private void Start() => panel.SetActive(false);

    public void ShowPanel(string weaponId)
    {
        currentWeaponId = weaponId;
        panel.SetActive(true);
        UpdateUI();
    }

    void UpdateUI()
    {
        int maxLevel = WeaponUpgradeSystem.Instance.GetMaxLevel(currentWeaponId);

        int level = WeaponUpgradeSystem.Instance.GetLevel(currentWeaponId);
        float currentDamage = WeaponUpgradeSystem.Instance.GetBaseDamage(currentWeaponId);
        float nextDamage = WeaponUpgradeSystem.Instance.GetNextBaseDamage(currentWeaponId);
        float percent = currentDamage > 0 ? ((nextDamage - currentDamage) / currentDamage) * 100f : 0f;

        float atkMod = WeaponUpgradeSystem.Instance.GetModifier(currentWeaponId, "stat.Attack_Damage_modifier");
        float magMod = WeaponUpgradeSystem.Instance.GetModifier(currentWeaponId, "stat.Magic_Damage_modifier");

        int cost = WeaponUpgradeSystem.Instance.GetCost(currentWeaponId);
        int coins = GameManager.Instance.GetCoins();

        currentLevelText.text = $"Level {level}";

        if (level >= maxLevel)
        {
            nextLevelText.text = $"Max Level";
        }
        else
        {
            nextLevelText.text = $"Level {level + 1}";
        }

        currentValueText.text = $"{currentDamage}";
        nextValueText.text = $"{nextDamage}"; // <color=green>(+{percent:F1}%)</color>"
        costText.text = $"{cost}";
        coinsText.text = $"{coins}";

        // Możesz dodać osobne pola TMP_Text na modyfikatory:
        // attackModText.text = $"{atkMod}% Attack";
        // magicModText.text = $"{magMod}% Magic";
    }

    public void OnUpgradeClick()
    {
        bool success = WeaponUpgradeSystem.Instance.TryUpgrade(currentWeaponId);
        if (success)
            UpdateUI();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}