using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDefenses : MonoBehaviour
{
    public float Armor;
    public float Magic_Resist;

    void Awake()
    {
        Armor = GetStatValues("Armor");
        Magic_Resist = GetStatValues("Magic_Resist");
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => StatSystem.Instance != null && StatSystem.Instance.IsReady);
    }

    public float GetStatValues(string statName)
    {
        if (statName == "Armor")
        {
            return Armor = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Magic_Resist")
        {
            return Magic_Resist = StatSystem.Instance.GetStatValue(statName);
        }
        else
        {
            return 0;
        }
    }

    public float GetDamageByDamageType(float damage, string damageType)
    {
        if (damageType == "Physical")
        {
            return damage - Armor;
        }
        else if (damageType == "Magic")
        {
            return damage - Magic_Resist;
        }
        else return damage;
    }

    public float GetArmor()
    {
        return Armor;
    }

    public float GetMagic_Resist()
    {
        return Magic_Resist;
    }
}
