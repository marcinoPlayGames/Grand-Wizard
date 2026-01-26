using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StatSystem : MonoBehaviour
{
    public static StatSystem Instance;

    [System.Serializable]
    public class StatData
    {
        public Dictionary<string, float> values;
        public Dictionary<string, int> costs;

        public void ResetDictionaries()
        {
            values.Clear();
            costs.Clear();
        }
    }

    public bool IsReady { get; private set; }

    // Zawiera wszystkie statystyki np. Attack_Damage, Magic_Attack
    private Dictionary<string, StatData> statTable;

    // Przechowuje poziomy gracza
    private Dictionary<string, int> playerLevels = new Dictionary<string, int>();

    public void ResetDictionaries()
    {
        playerLevels.Clear();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadStatsTable();
        LoadPlayerProgress();
    }

    private void Start()
    {
        IsReady = true;
    }

    void LoadStatsTable()
    {
        TextAsset json = Resources.Load<TextAsset>("JSON Files/StatisticsData/stats");

        statTable = JsonConvert.DeserializeObject<Dictionary<string, StatData>>(json.text);
    }

    public int GetLevel(string statName)
    { 
        return playerLevels.ContainsKey(statName) ? playerLevels[statName] : 0;
    }

    public float GetStatValue(string statName)
    {
        if (statTable == null)
        {
            return 0;
        }

        if (!statTable.ContainsKey(statName))
        {
            return 0;
        }

        int level = GetLevel(statName);

        return statTable[statName].values.ContainsKey(level.ToString()) ? statTable[statName].values[level.ToString()] : 0;
    }

    public float GetStatValueByLevel(string statName, int level)
    {
        return statTable[statName].values.ContainsKey(level.ToString())
            ? statTable[statName].values[level.ToString()]
            : 0;
    }

    public bool HasNextValue(string statName, int nextLevel)
    {
        return statTable.ContainsKey(statName) && statTable[statName].values.ContainsKey(nextLevel.ToString());
    }

    public int GetCost(string statName, int nextLevel)
    {
        return statTable[statName].costs.ContainsKey(nextLevel.ToString()) ? statTable[statName].costs[nextLevel.ToString()] : 0;
    }

    public UpgradeResult TryUpgrade(string statName)
    {
        if (!statTable.ContainsKey(statName))
            return UpgradeResult.InvalidStat;

        int currentLevel = GetLevel(statName);
        int nextLevel = currentLevel + 1;

        if (!statTable[statName].values.ContainsKey(nextLevel.ToString()))
            return UpgradeResult.NoNextLevel;

        int cost = GetCost(statName, nextLevel);
        if (cost < 0) return UpgradeResult.InvalidStat;

        if (GameManager.Instance.HasEnoughCoins(cost))
        {
            GameManager.Instance.SpendCoins(cost);
            playerLevels[statName] = nextLevel;
            SavePlayerProgress();

            LevelAnalytics.upgradesData newUpgrade = new LevelAnalytics.upgradesData();
            newUpgrade.upgrade_id = statName;

            if (statName == "Attack_Speed" || statName == "Spell_Speed")
            {
                newUpgrade.category = LevelAnalytics.upgradesData.upgradeCategory.speed;
            }
            
            else if (statName == "HP" || statName == "HP_Regen")
            {
                newUpgrade.category = LevelAnalytics.upgradesData.upgradeCategory.health;
            }
            else if (statName == "Mana" || statName == "Mana_Regen")
            {
                newUpgrade.category = LevelAnalytics.upgradesData.upgradeCategory.mana;
            }
            else if (statName == "Armor" || statName == "Magic_Resist")
            {
                newUpgrade.category = LevelAnalytics.upgradesData.upgradeCategory.resistances;
            }
            else
            {
                newUpgrade.category = LevelAnalytics.upgradesData.upgradeCategory.damage;
            }


            newUpgrade.total_cost += GetCost(statName, currentLevel);
            newUpgrade.level_after_purchase = currentLevel;
            newUpgrade.total_spent_treasures += GetCost(statName, currentLevel);

            LevelAnalytics.Instance.upgradesDatas.Add(newUpgrade);

            return UpgradeResult.Success;
        }

        return UpgradeResult.NotEnoughCoins;
    }

    const string SaveKey = "PlayerStats";

    void SavePlayerProgress()
    {
        string json = JsonConvert.SerializeObject(playerLevels);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void SaveGameRuntime()
    {
        SavePlayerProgressToFile();
    }
    
    public void SavePlayerGame()
    {
        SavePlayerProgressToFile(false);
    }
    void SavePlayerProgressToFile(bool runtime = true)
    {
        if (runtime) SavingSystem.SaveGameRuntime(playerLevels, GameManager.Instance.GetCoins());
        else SavingSystem.SavePlayerGame(playerLevels, GameManager.Instance.GetCoins());
    }

    void LoadPlayerProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            playerLevels = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }
    }

    public void LoadFromData(Dictionary<string, int> levels)
    {
        playerLevels = levels;
    }

    public void LoadGameButton()
    {
        SavingSystem.LoadPlayerGame();

        // Przełącz scenę, np. do głównego huba/świata
        UnityEngine.SceneManagement.SceneManager.LoadScene("NextLevel"); // lub "LevelSelect", jak wolisz
    }

    public List<string> GetAllStatNames()
    {
        return new List<string>(statTable.Keys);
    }

    public void ResetAllStatData()
    {
        foreach (var stat in statTable.Values)
        {
            stat.ResetDictionaries();
        }
    }
}