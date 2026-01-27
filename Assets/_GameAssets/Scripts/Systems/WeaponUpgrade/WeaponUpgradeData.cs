using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WeaponUpgradeData
{
    // Przykład innych właściwości
    public Dictionary<string, int> Cost { get; set; }
    public Dictionary<string, int> Base_Damage { get; set; }

    public Dictionary<string, float> Mana_Base { get; set; }
    public Dictionary<string, float> Mana_Ability { get; set; }
    public Dictionary<string, float> Attack_Speed { get; set; }
    public Dictionary<string, float> Spell_Speed { get; set; }
    public Dictionary<string, float> Attack_Range { get; set; }

    [JsonExtensionData]
    public Dictionary<string, Dictionary<string, float>> StatModifiers;
}