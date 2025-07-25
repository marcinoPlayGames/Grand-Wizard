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

    public string GetModifiersIcons(string weaponId, bool bGetNextLevel)
    {
        var data = LoadWeaponData(weaponId);
        if (data == null || data.StatModifiers == null)
            return "";

        int currentLevel = GetLevel(weaponId);
        int maxLevel = GetMaxLevel(weaponId);

        // Jeśli prosimy o next level, ale już jesteśmy na max – to zostajemy przy aktualnym
        int level = bGetNextLevel && currentLevel < maxLevel ? currentLevel + 1 : currentLevel;

        var sb = new System.Text.StringBuilder();

        // 👇 Dodajemy ikonkę Base_Damage jeśli jej wartość > 0
        if (data.Base_Damage != null && data.Base_Damage.TryGetValue(level.ToString(), out var baseDamage) && baseDamage > 0f)
        {
            sb.Append("<sprite name=\"Base_Damage\">");
        }

        // Pobieramy wszystkie statystyki z StatSystem
        var allStats = StatSystem.Instance.GetAllStatNames();

        foreach (var stat in allStats)
        {
            string key = $"stat.{stat}_modifier";

            if (data.StatModifiers.TryGetValue(key, out var jToken))
            {
                var dict = jToken.ToObject<Dictionary<string, float>>();
                if (dict != null && dict.TryGetValue(level.ToString(), out var val) && val != 0f)
                {
                    sb.Append($"<sprite name=\"{stat}\">");
                }
            }
        }

        return sb.ToString();
    }

    public float GetTotalDamage(string weaponId, bool bGetNextLevel)
    {
        var data = LoadWeaponData(weaponId);
        if (data == null)
            return 0f;

        int currentLevel = GetLevel(weaponId);
        int maxLevel = GetMaxLevel(weaponId);

        int level = bGetNextLevel && currentLevel < maxLevel ? currentLevel + 1 : currentLevel;
        float total = 0f;

        // 1. Base Damage
        if (data.Base_Damage != null && data.Base_Damage.TryGetValue(level.ToString(), out var baseDamage))
        {
            total += baseDamage;
        }

        // 2. Stat modifiers × stat values
        foreach (var statName in StatSystem.Instance.GetAllStatNames())
        {
            string modifierKey = $"stat.{statName}_modifier";

            if (data.StatModifiers.TryGetValue(modifierKey, out var jToken))
            {
                var dict = jToken.ToObject<Dictionary<string, float>>();
                if (dict != null && dict.TryGetValue(level.ToString(), out var weaponMod))
                {
                    int statLevel = StatSystem.Instance.GetLevel(statName);
                    float statValue = StatSystem.Instance.GetStatValue(statName);

                    total += statValue * (weaponMod / 100);
                }
            }
        }

        return Mathf.CeilToInt(total);
    }

    public float GetWeaponStatFloat(string weaponId, string statKey, bool bGetNextLevel)
    {
        var data = LoadWeaponData(weaponId);
        int currentLevel = GetLevel(weaponId);
        int maxLevel = GetMaxLevel(weaponId);
        int level = bGetNextLevel && currentLevel < maxLevel ? currentLevel + 1 : currentLevel;

        if (data == null)
            return 0f;

        Dictionary<string, float> dict = null;

        switch (statKey)
        {
            case "Mana_Base": dict = data.Mana_Base; break;
            case "Mana_Ability": dict = data.Mana_Ability; break;
            case "Attack_Speed": dict = data.Attack_Speed; break;
            case "Spell_Speed": dict = data.Spell_Speed; break;
            case "Attack_Range": dict = data.Attack_Range; break;
            default: return 0f;
        }

        return dict != null && dict.TryGetValue(level.ToString(), out float val) ? val : 0f;
    }

    public string GetWeaponDamageDetailsExtended(string weaponId, bool bGetNextLevel)
    {
        var data = LoadWeaponData(weaponId);
        if (data == null) return "";

        int currentLevel = GetLevel(weaponId);
        int maxLevel = GetMaxLevel(weaponId);
        int level = bGetNextLevel && currentLevel < maxLevel ? currentLevel + 1 : currentLevel;

        float baseDamage = GetBaseDamage(weaponId);
        float totalDamage = GetTotalDamage(weaponId, bGetNextLevel);
        float attackRange = GetWeaponStatFloat(weaponId, "Attack_Range", bGetNextLevel);
        float attackSpeedRaw = GetWeaponStatFloat(weaponId, "Attack_Speed", bGetNextLevel);
        float spellSpeedRaw = GetWeaponStatFloat(weaponId, "Spell_Speed", bGetNextLevel);
        float manaBase = GetWeaponStatFloat(weaponId, "Mana_Base", bGetNextLevel);
        float manaAbility = GetWeaponStatFloat(weaponId, "Mana_Ability", bGetNextLevel);

        float stat_AttackSpeed = StatSystem.Instance.GetStatValue("Attack_Speed");
        float stat_SpellSpeed = StatSystem.Instance.GetStatValue("Spell_Speed");
        float stat_Range = StatSystem.Instance.GetStatValue("Attack_Range");

        // Final values
        float finalRange = attackRange + stat_Range;
        float finalAttackSpeed = attackSpeedRaw * (1f - stat_AttackSpeed / 100f);
        float finalSpellSpeed = spellSpeedRaw * (1f - stat_SpellSpeed / 100f);

        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"Base Damage: {baseDamage:F0}");
        sb.AppendLine($"Total Damage: {totalDamage:F0}");
        sb.AppendLine($"Attack Range: {finalRange:F0}");
        sb.AppendLine($"Attack Cooldown: {finalAttackSpeed:F2}s");
        sb.AppendLine($"Ability Cooldown: {finalSpellSpeed:F2}s");
        sb.AppendLine($"Mana (Base Attack): {manaBase}");
        sb.AppendLine($"Mana (Ability): {manaAbility}");

        // Modyfikatory (jak poprzednio)
        float totalModifiers = totalDamage - baseDamage;
        var modifierList = new List<string>();

        foreach (var stat in StatSystem.Instance.GetAllStatNames())
        {
            string key = $"stat.{stat}_modifier";
            if (data.StatModifiers.TryGetValue(key, out var jToken))
            {
                var dict = jToken.ToObject<Dictionary<string, float>>();
                if (dict != null && dict.TryGetValue(level.ToString(), out var modValue) && modValue != 0f)
                {
                    float statVal = StatSystem.Instance.GetStatValue(stat);
                    float contribution = statVal * modValue;
                    if (totalModifiers > 0f)
                    {
                        float percent = (contribution / totalModifiers) * 100f;
                        modifierList.Add($"{percent:F0}% <sprite name=\"{stat}\">");
                    }
                }
            }
        }

        if (modifierList.Count > 0)
        {
            sb.AppendLine("Modifiers: " + string.Join(", ", modifierList));
        }

        return sb.ToString();
    }

    public float GetWeaponCostsCalculated(string weaponId, string statKey, bool bGetNextLevel)
    {
        float attackRange = GetWeaponStatFloat(weaponId, "Attack_Range", bGetNextLevel);
        float attackSpeedRaw = GetWeaponStatFloat(weaponId, "Attack_Speed", bGetNextLevel);
        float spellSpeedRaw = GetWeaponStatFloat(weaponId, "Spell_Speed", bGetNextLevel);
        float manaBase = GetWeaponStatFloat(weaponId, "Mana_Base", bGetNextLevel);
        float manaAbility = GetWeaponStatFloat(weaponId, "Mana_Ability", bGetNextLevel);

        float stat_AttackSpeed = StatSystem.Instance.GetStatValue("Attack_Speed");
        float stat_SpellSpeed = StatSystem.Instance.GetStatValue("Spell_Speed");
        float stat_Range = StatSystem.Instance.GetStatValue("Attack_Range");

        // Final values
        float finalRange = attackRange + stat_Range;
        float finalAttackSpeed = attackSpeedRaw * (1f - stat_AttackSpeed / 100f);
        float finalSpellSpeed = spellSpeedRaw * (1f - stat_SpellSpeed / 100f);

        if (statKey == "Attack_Speed")
        {
            return finalAttackSpeed;
        }
        else if (statKey == "Attack_Range")
        {
            return finalRange;
        }
        else if (statKey == "Spell_Speed")
        {
            return finalSpellSpeed;
        }
        else if (statKey == "Mana_Base")
        {
            return manaBase;
        }
        else if (statKey == "Mana_Ability")
        {
            return manaAbility;
        }
        else return finalAttackSpeed;
    }

    public string GetWeaponBasicAttackDetails(string weaponId, bool bGetNextLevel)
    {
        var data = LoadWeaponData(weaponId);
        if (data == null) return "";

        int currentLevel = GetLevel(weaponId);
        int maxLevel = GetMaxLevel(weaponId);
        int level = bGetNextLevel && currentLevel < maxLevel ? currentLevel + 1 : currentLevel;

        float baseDamage = GetBaseDamage(weaponId);
        float totalDamage = GetTotalDamage(weaponId, bGetNextLevel);
        float attackRange = GetWeaponStatFloat(weaponId, "Attack_Range", bGetNextLevel);
        float attackSpeedRaw = GetWeaponStatFloat(weaponId, "Attack_Speed", bGetNextLevel);
        float manaBase = GetWeaponStatFloat(weaponId, "Mana_Base", bGetNextLevel);

        string totalDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(totalDamage, WeaponUITextFormatter.DamageType.Magic);

        string levelIcons = WeaponUpgradeSystem.Instance.GetModifiersIcons(weaponId, bGetNextLevel);

        float stat_AttackSpeed = StatSystem.Instance.GetStatValue("Attack_Speed");
        float stat_Range = StatSystem.Instance.GetStatValue("Attack_Range");
        float stat_CritDamage = StatSystem.Instance.GetStatValue("Critical_Damage_Spell");

        float finalRange = attackRange + stat_Range;
        float finalAttackSpeed = attackSpeedRaw * (1f - stat_AttackSpeed / 100f);
        float finalCritTotalDamage = Mathf.CeilToInt(totalDamage * (stat_CritDamage / 100f));

        string critTotalDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(finalCritTotalDamage, WeaponUITextFormatter.DamageType.Magic);

        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"Base Damage <sprite name=\"Base_Damage\">: {baseDamage:F0}");
        sb.AppendLine($"Total Damage ({levelIcons}): {totalDamage_colored:F0}");
        sb.AppendLine($"Crit Total Damage (<sprite name=\"Critical_Damage_Spell\">{levelIcons}): <sprite name=\"Critical_Chance_Spell\">{critTotalDamage_colored:F0}");
        sb.AppendLine($"Attack Range <sprite name=\"Attack_Range\">: {finalRange:F0}");
        sb.AppendLine($"Attack Speed <sprite name=\"Attack_Speed\">: {finalAttackSpeed:F2}s");
        sb.AppendLine($"Mana Cost <sprite name=\"Mana\">: {manaBase}");

        // Oblicz udział modyfikatorów

        var modifierList = new List<string>();

        foreach (var stat in StatSystem.Instance.GetAllStatNames())
        {
            string key = $"stat.{stat}_modifier";
            if (data.StatModifiers.TryGetValue(key, out var jToken))
            {
                var dict = jToken.ToObject<Dictionary<string, float>>();
                if (dict != null && dict.TryGetValue(level.ToString(), out var modValue) && modValue != 0f)
                {
                    float statVal = StatSystem.Instance.GetStatValue(stat);
                    float contribution = statVal * (modValue / 100f);

                    // Tu zamiast wyliczać % udziału, pokazujemy wizualny % i wkład
                    modifierList.Add($"{modValue:F0}% ({contribution:F0}) <sprite name=\"{stat}\">");
                }
            }
        }

        if (modifierList.Count > 0)
        {
            sb.AppendLine("Modifiers: " + string.Join(", ", modifierList));
        }

        return sb.ToString();
    }

    public string GetWeaponAbilityAttackDetails(string weaponId, bool bGetNextLevel)
    {
        var data = LoadWeaponData(weaponId);
        if (data == null) return "";

        int currentLevel = GetLevel(weaponId);
        int maxLevel = GetMaxLevel(weaponId);
        int level = bGetNextLevel && currentLevel < maxLevel ? currentLevel + 1 : currentLevel;

        float baseDamage = GetBaseDamage(weaponId);
        float totalDamage = GetTotalDamage(weaponId, bGetNextLevel);

        string totalDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(totalDamage * 2, WeaponUITextFormatter.DamageType.Magic);

        float attackRange = GetWeaponStatFloat(weaponId, "Attack_Range", bGetNextLevel);
        float spellSpeedRaw = GetWeaponStatFloat(weaponId, "Spell_Speed", bGetNextLevel);
        float manaAbility = GetWeaponStatFloat(weaponId, "Mana_Ability", bGetNextLevel);

        float stat_SpellSpeed = StatSystem.Instance.GetStatValue("Spell_Speed");
        float stat_Range = StatSystem.Instance.GetStatValue("Attack_Range");
        float stat_CritDamage = StatSystem.Instance.GetStatValue("Critical_Damage_Spell");

        float finalRange = attackRange + stat_Range;
        float finalSpellSpeed = spellSpeedRaw * (1f - stat_SpellSpeed / 100f);
        float finalCritTotalDamage = Mathf.CeilToInt(totalDamage * 2 * (stat_CritDamage / 100f));

        string critTotalDamage_colored = WeaponUITextFormatter.GetColoredRawDamageText(finalCritTotalDamage, WeaponUITextFormatter.DamageType.Magic);

        string levelIcons = WeaponUpgradeSystem.Instance.GetModifiersIcons(weaponId, bGetNextLevel);

        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"Base Damage <sprite name=\"Base_Damage\">: {2 * baseDamage:F0}");
        sb.AppendLine($"Total Damage ({levelIcons}): {totalDamage_colored:F0}");
        sb.AppendLine($"Crit Total Damage (<sprite name=\"Critical_Damage_Spell\">{levelIcons}): <sprite name=\"Critical_Chance_Spell\">{critTotalDamage_colored:F0}");
        sb.AppendLine($"Ability Range <sprite name=\"Attack_Range\">: {finalRange:F0}");
        sb.AppendLine($"Ability Speed <sprite name=\"Spell_Speed\">: {finalSpellSpeed:F2}s");
        sb.AppendLine($"Mana Cost <sprite name=\"Mana\">: {manaAbility}");

        // Oblicz udział modyfikatorów

        var modifierList = new List<string>();

        foreach (var stat in StatSystem.Instance.GetAllStatNames())
        {
            string key = $"stat.{stat}_modifier";
            if (data.StatModifiers.TryGetValue(key, out var jToken))
            {
                var dict = jToken.ToObject<Dictionary<string, float>>();
                if (dict != null && dict.TryGetValue(level.ToString(), out var modValue) && modValue != 0f)
                {
                    float statVal = StatSystem.Instance.GetStatValue(stat);
                    float contribution = statVal * (modValue / 100f);

                    // Tu zamiast wyliczać % udziału, pokazujemy wizualny % i wkład
                    modifierList.Add($"{modValue:F0}% ({contribution:F0}) <sprite name=\"{stat}\">");
                }
            }
        }

        if (modifierList.Count > 0)
        {
            sb.AppendLine("Modifiers: " + string.Join(", ", modifierList));
        }

        return sb.ToString();
    }

    public Dictionary<string, int> GetWeaponLevels()
    {
        return new Dictionary<string, int>(weaponLevels); // zakładam, że masz ten słownik prywatny
    }

    public void LoadWeaponLevels(Dictionary<string, int> loadedLevels)
    {
        weaponLevels = loadedLevels ?? new Dictionary<string, int>();
    }
}