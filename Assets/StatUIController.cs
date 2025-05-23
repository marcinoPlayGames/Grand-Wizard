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
    private string currentStat;

    private void Awake() => Instance = this;

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
        statNameText.text = currentStat;
        costText.text = $"Cena: {cost}";
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