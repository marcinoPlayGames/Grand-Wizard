using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticals : MonoBehaviour
{
    public int Attack_Critical_Hit_Chance;
    public int Attack_Critical_Hit_Damage;

    public int Spell_Critical_Hit_Chance;
    public int Spell_Critical_Hit_Damage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsAttackCriticalHit()
    {
        int critChance = Attack_Critical_Hit_Chance;
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < critChance;
    }

    public bool IsSpellCriticalHit()
    {
        int critChance = Spell_Critical_Hit_Chance;
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < critChance;
    }

    public int GetCriticalDamageByAttackType(int damage, string attackType)
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
