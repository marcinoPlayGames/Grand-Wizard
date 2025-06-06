using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticals : MonoBehaviour
{
    public float Attack_Critical_Hit_Chance;
    public float Attack_Critical_Hit_Damage;

    public float Spell_Critical_Hit_Chance;
    public float Spell_Critical_Hit_Damage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetStatValues(string statName)
    {
        if (statName == "Attack_Critical_Hit_Chance")
        {
            return Attack_Critical_Hit_Chance = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Attack_Critical_Hit_Damage")
        {
            return Attack_Critical_Hit_Damage = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Spell_Critical_Hit_Chance")
        {
            return Spell_Critical_Hit_Chance = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Spell_Critical_Hit_Damage")
        {
            return Spell_Critical_Hit_Damage = StatSystem.Instance.GetStatValue(statName);
        }
        else
        {
            return 0;
        }
    }

    public bool IsAttackCriticalHit()
    {
        float critChance = Attack_Critical_Hit_Chance;
        float roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < critChance;
    }

    public bool IsSpellCriticalHit()
    {
        float critChance = Spell_Critical_Hit_Chance;
        float roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < critChance;
    }

    public float GetCriticalDamageByAttackType(float damage, string attackType)
    {
        if (attackType == "Physical")
        {
            return damage * Attack_Critical_Hit_Damage;
        }
        else if (attackType == "Ability" || attackType == "Spell")
        {
            return damage * Spell_Critical_Hit_Damage;
        }
        else return damage * Attack_Critical_Hit_Damage;
    }
}
