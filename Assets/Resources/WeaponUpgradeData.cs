using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WeaponUpgradeData
{
    // Przykład innych właściwości
    public Dictionary<string, int> Cost { get; set; }
    public Dictionary<string, int> Base_Damage { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken> StatModifiers { get; set; }
}