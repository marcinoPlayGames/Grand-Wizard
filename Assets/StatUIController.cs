using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private string currentStat;

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

        Debug.Log(GameManager.Instance.GetCoins());
        statNameText.text = currentStat;
        costText.text = $"Cena: {cost}";
        nextLevel.text = $"{level + 1}";
        currentLevel.text = $"{level}";
        coinAmount.text = $"{coins}";
    }

    public void OnUpgradeButton()
    {
        StatSystem.Instance.TryUpgrade(currentStat);
        UpdateUpgradeUI();
    }

    public void CloseUpgradeWindow()
    {
        upgradePanel.SetActive(false);
        treePanel.SetActive(true);
    }
}