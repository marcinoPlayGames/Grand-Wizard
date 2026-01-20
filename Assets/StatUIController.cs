using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StatUIController : MonoBehaviour
{
    public static StatUIController Instance;

    public GameObject treePanel;
    public GameObject upgradePanel;
    public TextMeshProUGUI statNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI nextLevel;
    public TextMeshProUGUI coinAmount;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI nextValueText;
    public Image statIconImage;
    private string currentStat;
    public UIButtonInfoHandler uiButtonInfoHandler;

    private void Awake() => Instance = this;

    void Start()
    {
        CloseUpgradeWindow();
    }

    public void ShowUpgradeWindow(string statId)
    {
        currentStat = statId;
        treePanel.SetActive(false);
        upgradePanel.SetActive(true);
        UpdateUpgradeUI();
    }


    void UpdateUpgradeUI()
    {
        int level = StatSystem.Instance.GetLevel(currentStat);
        int cost = StatSystem.Instance.GetCost(currentStat, level + 1);
        int coins = GameManager.Instance.GetCoins();

        string nameCleared = currentStat.Replace("_", " ");

        statNameText.text = nameCleared;
        costText.text = $"{cost}";
        if (level + 1 >= 11)
        {
            nextLevel.text = $"Max";
        }
        else
        {
            nextLevel.text = $"{level + 1}";
        }
        
        currentLevel.text = $"{level}";
        coinAmount.text = $"{coins}";

        float currentValue = StatSystem.Instance.GetStatValue(currentStat);
        float nextValue = StatSystem.Instance.HasNextValue(currentStat, level + 1)
            ? StatSystem.Instance.GetStatValueByLevel(currentStat, level + 1)
            : currentValue;

        string additionalSign = "";

        if (currentStat == "Attack_Speed" || currentStat == "Spell_Speed" || currentStat == "Critical_Chance_Spell" || currentStat == "Critical_Chance_Attack" || currentStat == "Critical_Damage_Attack" || currentStat == "Critical_Damage_Spell" || currentStat == "Healing")
        {
            additionalSign = "%";
        }
        else if (currentStat == "HP_Regen" || currentStat == "Mana_Regen")
        {
            additionalSign = "/s";
        }
        else
        {
            additionalSign = "";
        }

        currentValueText.text = $"{currentValue}{additionalSign}";
        nextValueText.text = $"{nextValue}{additionalSign}";

        Sprite icon = Resources.Load<Sprite>($"StatIcons/{currentStat}");
        if (icon != null)
        {
            statIconImage.sprite = icon;
        }
        else
        {
            Debug.LogWarning($"Brak ikony dla: {currentStat}");
        }
    }

    public void OnUpgradeButton()
    {
        UpgradeResult upgradeResult = StatSystem.Instance.TryUpgrade(currentStat);
        UpdateUpgradeUI();

        ShowStatisticUpgradeUIInfo(upgradeResult);
    }

    public void CloseUpgradeWindow()
    {
        upgradePanel.SetActive(false);
        treePanel.SetActive(true);
    }

    public void GoBackToNextLevel()
    {
        SceneManager.LoadScene("NextLevel");
    }

    private void ShowStatisticUpgradeUIInfo(UpgradeResult upgradeResult)
    {
        switch (upgradeResult)
        {
            case UpgradeResult.Success:
                uiButtonInfoHandler.ShowInfoUI("Upgrade was successful!");
                break;

            case UpgradeResult.NotEnoughCoins:
                uiButtonInfoHandler.ShowInfoUI("Not enough coins!", InfoType.Error);
                break;

            case UpgradeResult.NoNextLevel:
                uiButtonInfoHandler.ShowInfoUI("Upgrade already maxed!", InfoType.Warning);
                break;

            case UpgradeResult.InvalidStat:
                uiButtonInfoHandler.ShowInfoUI("Upgrade error!", InfoType.Error);
                break;
        }
    }
}