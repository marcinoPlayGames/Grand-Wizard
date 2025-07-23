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

        Debug.Log("Stats file loaded: " + (json != null));
        Debug.Log(json.text);

        statTable = JsonConvert.DeserializeObject<Dictionary<string, StatData>>(json.text);

        if (statTable.ContainsKey("HP"))
        {
            Debug.Log("HP stat loaded.");
            foreach (var kvp in statTable["HP"].values)
            {
                Debug.Log($"Level {kvp.Key}: Value {kvp.Value}");
            }
        }
    }

    public int GetLevel(string statName)
    {
        if (statName == "HP")
        {
            Debug.Log("HP lvl = ");
            Debug.Log(playerLevels.ContainsKey(statName));
            Debug.Log(statTable[statName].values.ContainsKey(0.ToString()));
            Debug.Log(statTable[statName].values[0.ToString()]);
        }
        return playerLevels.ContainsKey(statName) ? playerLevels[statName] : 0;
    }

    public float GetStatValue(string statName)
    {
        int level = GetLevel(statName);

        Debug.Log("level = " + level);

        Debug.Log(statTable[statName].values.ContainsKey(level.ToString()));
        Debug.Log(statTable[statName].values[level.ToString()]);
        Debug.Log(statTable[statName].values.ContainsKey(level.ToString()) ? statTable[statName].values[level.ToString()] : 0);
        return statTable[statName].values.ContainsKey(level.ToString()) ? statTable[statName].values[level.ToString()] : 0;
    }

    public int GetCost(string statName, int nextLevel)
    {
        return statTable[statName].costs.ContainsKey(nextLevel.ToString()) ? statTable[statName].costs[nextLevel.ToString()] : -1;
    }

    public bool TryUpgrade(string statName)
    {
        int currentLevel = GetLevel(statName);
        int nextLevel = currentLevel + 1;

        if (!statTable[statName].values.ContainsKey(nextLevel.ToString()))
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

    public void SaveGame()
    {
        SavePlayerProgressToFile();
    }
    
    void SavePlayerProgressToFile()
    {
        SavingSystem.SaveGame(playerLevels, GameManager.Instance.GetCoins());
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
        SavingSystem.LoadLatestGame();

        // Przełącz scenę, np. do głównego huba/świata
        UnityEngine.SceneManagement.SceneManager.LoadScene("NextLevel"); // lub "LevelSelect", jak wolisz
    }

    public List<string> GetAllStatNames()
    {
        return new List<string>(statTable.Keys);
    }
}