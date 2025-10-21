using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticals : MonoBehaviour
{
    public float Attack_Critical_Hit_Chance;
    public float Attack_Critical_Hit_Damage;

    public float Spell_Critical_Hit_Chance;
    public float Spell_Critical_Hit_Damage;

    void Awake()
    {
        Attack_Critical_Hit_Chance = GetStatValues("Critical_Chance_Attack");
        Attack_Critical_Hit_Damage = GetStatValues("Critical_Damage_Attack");
        Spell_Critical_Hit_Chance = GetStatValues("Critical_Chance_Spell");
        Spell_Critical_Hit_Damage = GetStatValues("Critical_Damage_Spell");
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => StatSystem.Instance != null && StatSystem.Instance.IsReady);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetStatValues(string statName)
    {
        if (statName == "Critical_Chance_Attack")
        {
            return Attack_Critical_Hit_Chance = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Critical_Damage_Attack")
        {
            return Attack_Critical_Hit_Damage = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Critical_Chance_Spell")
        {
            return Spell_Critical_Hit_Chance = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Critical_Damage_Spell")
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
            return Mathf.CeilToInt(damage * (Attack_Critical_Hit_Damage / 100));
        }
        else if (attackType == "Ability" || attackType == "Spell")
        {
            return Mathf.CeilToInt(damage * (Spell_Critical_Hit_Damage / 100));
        }
        else return damage;
    }
}
