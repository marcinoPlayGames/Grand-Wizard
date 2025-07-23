using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    public TMP_Text damageText;

    void Start()
    {
        int dmg = 20;
        var damageString = WeaponUITextFormatter.GetColoredDamageText(dmg, WeaponUITextFormatter.DamageType.Magic);
        damageText.text = damageString;
    }
}