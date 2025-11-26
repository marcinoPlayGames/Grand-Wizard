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
        Debug.Log($"[StatSystem] Awake on {gameObject.name} (ID={GetInstanceID()})"); 



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
        Debug.Log("[StatSystem] Stats fully loaded.");
    }

    void LoadStatsTable()
    {
        Debug.Log($"[StatSystemLoad] Awake on {gameObject.name} (ID={GetInstanceID()})");

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

        foreach (var kvp in statTable)
        {
            var stat = kvp.Key;
            var values = kvp.Value.values;
            var costs = kvp.Value.costs;

            Debug.Log($"{stat}: values={values.Count}, costs={costs.Count}, has0={values.ContainsKey("0")}, level0={values["0"]}");
        }
    }

    public int GetLevel(string statName)
    {
        if (statName == "HP")
        {
            Debug.Log("[HP] HP lvl = ");
            Debug.Log("[HP] " + playerLevels.ContainsKey(statName));
            Debug.Log("[HP] " + statTable[statName].values.ContainsKey(0.ToString()));
            //Debug.Log("[HP] " + statTable[statName].values[0.ToString()]);
        }
        return playerLevels.ContainsKey(statName) ? playerLevels[statName] : 0;
    }

    public float GetStatValue(string statName)
    {
        Debug.Log($"[StatSystem-GetStatValue] Current Instance ID={StatSystem.Instance.GetInstanceID()}, statTable={(StatSystem.Instance.statTable != null ? StatSystem.Instance.statTable.Count.ToString() : "null")}");

        Debug.Log($"[GetStatValue] statName = '{statName}'");
        Debug.Log($"[GetStatValue] Keys in statTable: {string.Join(", ", statTable.Keys)}");

        if (statTable == null)
        {
            Debug.LogWarning("StatSystem not initialized yet, returning 0 for " + statName);
            return 0;
        }

        if (!statTable.ContainsKey(statName))
        {
            Debug.LogError($"Stat '{statName}' not found in statTable!");
            return 0;
        }

        int level = GetLevel(statName);

        Debug.Log("[GetStatValue] " + statName);
        Debug.Log("level = " + level);
        Debug.Log("[GetStatValue] stat value = " + statTable[statName].values["0"]);

        Debug.Log($"[HP-v] Keys in statTable for HP: {string.Join(", ", statTable["HP"].values.Keys)}");

        Debug.Log(statTable[statName].values.ContainsKey(level.ToString()));
        Debug.Log(statTable[statName].values[level.ToString()]);
        Debug.Log(statTable[statName].values.ContainsKey(level.ToString()) ? statTable[statName].values[level.ToString()] : 0);
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