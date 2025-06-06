using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAttacks : MonoBehaviour
{
    public float Attack_Damage;
    public float Magic_Damage;
    public float Attack_Speed;
    public float Spell_Speed;
    public float Attack_Range;
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
        if (statName == "Attack_Damage")
        {
            return Attack_Damage = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Magic_Damage")
        {
            return Magic_Damage = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Attack_Speed")
        {
            return Attack_Speed = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Spell_Speed")
        {
            return Spell_Speed = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Attack_Range")
        {
            return Attack_Range = StatSystem.Instance.GetStatValue(statName);
        }
        else
        {
            return 0;
        }
    }
    
    public float GetDamageValue(int damagePercent, string statisticType)
    {
        if (statisticType == "Attack")
        {
            return Attack_Damage * damagePercent;
        }
        else if (statisticType == "Magic")
        {
            return Magic_Damage * damagePercent;
        }
        else return 0;
    }

    public float GetDamageVariables(string statisticType)
    {
        if (statisticType == "Attack_Speed")
        {
            return Attack_Speed;
        }
        else if (statisticType == "Spell_Speed")
        {
            return Spell_Speed;
        }
        else if (statisticType == "Attack_Range")
        {
            return Attack_Range;
        }
        else return 0;
    }
}
