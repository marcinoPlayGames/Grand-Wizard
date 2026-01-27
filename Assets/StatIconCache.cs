using System.Collections.Generic;
using UnityEngine;

public class StatIconCache : MonoBehaviour
{
    public static StatIconCache Instance;

    private Dictionary<string, Sprite> icons = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadIcons();
        }
        else Destroy(gameObject);
    }

    void LoadIcons()
    {
        var loadedIcons = Resources.LoadAll<Sprite>("StatIcons");
        foreach (var icon in loadedIcons)
            icons[icon.name] = icon;
    }

    public Sprite GetIcon(string statName)
    {
        return icons.TryGetValue(statName, out var sprite) ? sprite : null;
    }
}