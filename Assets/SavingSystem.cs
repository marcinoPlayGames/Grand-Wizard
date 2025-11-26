using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public static class SavingSystem
{
    private static string gameName = "GrandWizard"; // Zmień na swoją nazwę gry

    private static string runtimeDataName = "data_runtime";
    private static string saveDataName = "player_save_data";

    public static string GetSaveDirectory()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), gameName, "Saves");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static string GetSaveFilePathRuntime()
    {
        return Path.Combine(GetSaveDirectory(), $"{runtimeDataName}.json");
    }

    public static string GetSaveFilePathPlayerSave()
    {
        return Path.Combine(GetSaveDirectory(), $"{saveDataName}.json");
    }

    public static void SaveGameRuntime(Dictionary<string, int> stats, int coins)
    {
        SaveData data = new SaveData
        {
            playerLevels = stats,
            coinCount = coins,
            unlockedLevel = GameManager.Instance.GetUnlockedLevel(),
            weaponLevels = WeaponUpgradeSystem.Instance.GetWeaponLevels() // <-- dodane
        };

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string filePath = GetSaveFilePathRuntime();
        File.WriteAllText(filePath, json);

        Debug.Log($"Zapisano grę do pliku: {filePath}");
    }

    public static void SavePlayerGame(Dictionary<string, int> stats, int coins)
    {
        SaveData data = LoadGame(GetSaveFilePathRuntime());

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string filePath = GetSaveFilePathPlayerSave();
        File.WriteAllText(filePath, json);

        Debug.Log($"Zapisano grę do pliku: {filePath}");
    }

    public static SaveData LoadGame(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Plik zapisu nie istnieje: {filePath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

        // Wczytanie danych do GameManager
        GameManager.Instance.coins = data.coinCount;
        GameManager.Instance.unlockedLevel = data.unlockedLevel;

        // Wczytanie danych do StatSystem
        StatSystem.Instance.LoadFromData(data.playerLevels);

        // Wczytanie danych do WeaponUpgradeSystem
        WeaponUpgradeSystem.Instance.LoadWeaponLevels(data.weaponLevels); // <-- dodane

        return data;
    }

    [Serializable]
    public class SaveData
    {
        public Dictionary<string, int> playerLevels;
        public int coinCount;
        public int unlockedLevel;
        public Dictionary<string, int> weaponLevels;

        public void LoadFromSaveFile(string filePath)
        {
            var data = SavingSystem.LoadGame(filePath);
            if (data != null)
            {
                playerLevels = data.playerLevels;
                GameManager.Instance.coins = data.coinCount;
                GameManager.Instance.unlockedLevel = data.unlockedLevel;

                // NOWOŚĆ: Załaduj poziomy broni
                WeaponUpgradeSystem.Instance.LoadWeaponLevels(data.weaponLevels);
            }
        }
    }

    public static string GetPlayerSaveGameFile()
    {
        return Directory.GetFiles(SavingSystem.GetSaveDirectory(), $"{saveDataName}.json").FirstOrDefault();
    }
    public static void LoadPlayerGame()
    {
        string file = GetPlayerSaveGameFile();
        if (string.IsNullOrEmpty(file))
        {
            Debug.LogWarning("Nie znaleziono pliku.");
            return;
        }

        Debug.Log($"Ładowanie zapisu z pliku: {file}");
        LoadGame(file);
    }

    public static bool DoesSaveGameFileExist()
    {
        string file = GetPlayerSaveGameFile();
        if (string.IsNullOrEmpty(file))
        {
            Debug.LogWarning("Nie znaleziono pliku.");
            return false;
        }

        return true;
    }
}