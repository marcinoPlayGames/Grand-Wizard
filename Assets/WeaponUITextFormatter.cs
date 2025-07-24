using TMPro;
using UnityEngine;

public static class WeaponUITextFormatter
{
    public enum DamageType
    {
        Physical,
        Magic,
        True // nieuchronne
    }

    public static string GetColoredDamageText(string textStart, float damageAmount, string damageIcons, string additionalLabel, DamageType type)
    {
        string colorHex;
        string damageLabel;

        switch (type)
        {
            case DamageType.Physical:
                colorHex = "8B4513"; // brązowy (saddle brown)
                damageLabel = "physical damage";
                break;
            case DamageType.Magic:
                colorHex = "00CED1"; // morski (dark turquoise)
                damageLabel = "magic damage";
                break;
            case DamageType.True:
                colorHex = "FFFFFF"; // biały
                damageLabel = "true damage";
                break;
            default:
                colorHex = "000000"; // czarny – fallback
                damageLabel = "damage";
                break;
        }

        return $"<color=black>{textStart} that deals </color><color=#{colorHex}>{damageAmount}</color><color=black> ({damageIcons}){additionalLabel} {damageLabel}</color>";
    }

    public static string GetColoredRawDamageText(float damageAmount, DamageType type)
    {
        string colorHex;

        switch (type)
        {
            case DamageType.Physical:
                colorHex = "8B4513"; // brązowy (saddle brown)
                break;
            case DamageType.Magic:
                colorHex = "00CED1"; // morski (dark turquoise)
                break;
            case DamageType.True:
                colorHex = "FFFFFF"; // biały           
                break;
            default:
                colorHex = "000000"; // czarny – fallback
                break;
        }

        return $"<color=#{colorHex}>{damageAmount}</color>";
    }
}