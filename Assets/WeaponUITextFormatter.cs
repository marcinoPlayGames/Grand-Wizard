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

    public static string GetColoredDamageText(int damageAmount, DamageType type)
    {
        string colorHex;
        string damageLabel;

        switch (type)
        {
            case DamageType.Physical:
                colorHex = "8B4513"; // brązowy (saddle brown)
                damageLabel = "obrażeń fizycznych";
                break;
            case DamageType.Magic:
                colorHex = "00CED1"; // morski (dark turquoise)
                damageLabel = "obrażeń magicznych";
                break;
            case DamageType.True:
                colorHex = "FFFFFF"; // biały
                damageLabel = "obrażeń nieuchronnych";
                break;
            default:
                colorHex = "000000"; // czarny – fallback
                damageLabel = "obrażeń";
                break;
        }

        return $"<color=black>Zadaje </color><color=#{colorHex}>{damageAmount}</color><color=black> {damageLabel}</color>";
    }
}