using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class WeaponUpgradeSystem : MonoBehaviour
{
    public static WeaponUpgradeSystem Instance;

    private Dictionary<string, WeaponUpgradeData> weaponDataCache = new Dictionary<string, WeaponUpgradeData>();
    private Dictionary<string, int> weaponLevels = new Dictionary<string, int>();

    const string SaveKey = "WeaponUpgrades";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public WeaponUpgradeData LoadWeaponData(string weaponId)
    {
        if (weaponDataCache.ContainsKey(weaponId))
            return weaponDataCache[weaponId];

        TextAsset json = Resources.Load<TextAsset>(weaponId); // np. "Staff"
        if (json == null)
        {
            Debug.LogError("Nie znaleziono pliku JSON dla broni: " + weaponId);
            return null;
        }

        WeaponUpgradeData data = JsonConvert.DeserializeObject<WeaponUpgradeData>(json.text);
        weaponDataCache[weaponId] = data;
        return data;
    }

    public int GetLevel(string weaponId)
    {
        return weaponLevels.ContainsKey(weaponId) ? weaponLevels[weaponId] : 0;
    }

    public int GetMaxLevel(string weaponId)
    {
        var data = LoadWeaponData(weaponId);
        int maxLevel = 0;
        foreach (var key in data.Cost.Keys)
        {
            if (int.TryParse(key, out int levelNum))
            {
                if (levelNum > maxLevel) maxLevel = levelNum;
            }
        }
        return maxLevel;
    }

    public int GetCost(string weaponId)
    {
        var data = LoadWeaponData(weaponId);
        int nextLevel = GetLevel(weaponId) + 1;
        return data.Cost.ContainsKey(nextLevel.ToString()) ? data.Cost[nextLevel.ToString()] : 0;
    }

    public float GetBaseDamage(string weaponId)
    {
        var data = LoadWeaponData(weaponId);
        int level = GetLevel(weaponId);
        return data.Base_Damage.ContainsKey(level.ToString()) ? data.Base_Damage[level.ToString()] : 0f;
    }

    public float GetNextBaseDamage(string weaponId)
    {
        var data = LoadWeaponData(weaponId);
        int level = GetLevel(weaponId) + 1;
        return data.Base_Damage.ContainsKey(level.ToString()) ? data.Base_Damage[level.ToString()] : GetBaseDamage(weaponId);
    }

    public float GetModifier(string weaponId, string statName)
    {
        var data = LoadWeaponData(weaponId);
        int level = GetLevel(weaponId);

        string key = $"stat.{statName}_modifier";

        if (data.StatModifiers.TryGetValue(key, out var jToken))
        {
            // Konwertujemy JToken na słownik
            var dict = jToken.ToObject<Dictionary<string, float>>();

            if (dict != null && dict.TryGetValue(level.ToString(), out var value))
            {
                return value;
            }
        }

        return 0f;
    }

    public bool TryUpgrade(string weaponId)
    {
        int nextLevel = GetLevel(weaponId) + 1;
        var data = LoadWeaponData(weaponId);
        if (!data.Base_Damage.ContainsKey(nextLevel.ToString())) return false;

        int cost = GetCost(weaponId);
        if (!GameManager.Instance.HasEnoughCoins(cost)) return false;

        GameManager.Instance.SpendCoins(cost);
        weaponLevels[weaponId] = nextLevel;
        SaveProgress();
        return true;
    }

    void SaveProgress()
    {
        string json = JsonConvert.SerializeObject(weaponLevels);
        PlayerPrefs.SetString(SaveKey, json);
    }

    void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            weaponLevels = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }
    }
}