using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StatSystem : MonoBehaviour
{
    public static StatSystem Instance;

    [System.Serializable]
    public class StatData
    {
        public Dictionary<int, float> values;
        public Dictionary<int, int> costs;
    }

    // Zawiera wszystkie statystyki np. Attack_Damage, Magic_Attack
    private Dictionary<string, StatData> statTable;

    // Przechowuje poziomy gracza
    private Dictionary<string, int> playerLevels = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStatsTable();
            LoadPlayerProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadStatsTable()
    {
        TextAsset json = Resources.Load<TextAsset>("stats");
        statTable = JsonConvert.DeserializeObject<Dictionary<string, StatData>>(json.text);
    }

    public int GetLevel(string statName)
    {
        return playerLevels.ContainsKey(statName) ? playerLevels[statName] : 1;
    }

    public float GetStatValue(string statName)
    {
        int level = GetLevel(statName);
        return statTable[statName].values.ContainsKey(level) ? statTable[statName].values[level] : 0;
    }

    public int GetCost(string statName, int nextLevel)
    {
        return statTable[statName].costs.ContainsKey(nextLevel) ? statTable[statName].costs[nextLevel] : -1;
    }

    public bool TryUpgrade(string statName)
    {
        int currentLevel = GetLevel(statName);
        int nextLevel = currentLevel + 1;

        if (!statTable[statName].values.ContainsKey(nextLevel))
            return false;

        int cost = GetCost(statName, nextLevel);
        if (cost < 0) return false;

        if (GameManager.Instance.HasEnoughCoins(cost))
        {
            GameManager.Instance.SpendCoins(cost);
            playerLevels[statName] = nextLevel;
            SavePlayerProgress();
            return true;
        }

        return false;
    }

    const string SaveKey = "PlayerStats";

    void SavePlayerProgress()
    {
        string json = JsonConvert.SerializeObject(playerLevels);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    void LoadPlayerProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            playerLevels = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }
    }
}