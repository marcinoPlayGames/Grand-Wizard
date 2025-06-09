using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class SavingSystem
{
    private static string gameName = "GrandWizard"; // Zmień na swoją nazwę gry

    public static string GetSaveDirectory()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), gameName, "Saves");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static string GetSaveFilePath()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return Path.Combine(GetSaveDirectory(), $"{timestamp}.json");
    }

    public static void SaveGame(Dictionary<string, int> stats, int coins)
    {
        SaveData data = new SaveData
        {
            playerLevels = stats,
            coinCount = coins
        };

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string filePath = GetSaveFilePath();
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
        return JsonConvert.DeserializeObject<SaveData>(json);
    }

    [Serializable]
    public class SaveData
    {
        public Dictionary<string, int> playerLevels;
        public int coinCount;

        public void LoadFromSaveFile(string filePath)
        {
            var data = SavingSystem.LoadGame(filePath);
            if (data != null)
            {
                playerLevels = data.playerLevels;
                GameManager.Instance.coins = data.coinCount;
            }
        }
    }  

    public static string[] GetAllSaveFiles()
    {
        return Directory.GetFiles(SavingSystem.GetSaveDirectory(), "*.json");
    }
}