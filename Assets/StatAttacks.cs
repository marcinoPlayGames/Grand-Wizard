using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAttacks : MonoBehaviour
{
    public int Attack_Damage;
    public int Magic_Damage;
    public int Attack_Speed;
    public int Spell_Speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetDamageValue(int damagePercent, string statisticType)
    {
        if (statisticType == "Attack")
        {
            return Attack_Damage * damage;
        }
        else if (statisticType == "Magic")
        {
            return Magic_Damage * damage;
        }
        else return 0;
    }

    public int GetDamageVariables(string statisticType)
    {
        if (statisticType == "Attack_Speed")
        {
            return Attack_Speed;
        }
        else if (statisticType == "Spell_Speed")
        {
            return Spell_Speed;
        }
    }
}
