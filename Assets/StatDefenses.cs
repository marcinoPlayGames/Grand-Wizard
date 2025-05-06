using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDefenses : MonoBehaviour
{
    public int Armor;
    public int MagicResist;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetDamageByDamageType(int damage, string damageType)
    {
        if (damageType == "Physical")
        {
            return damage - Armor;
        }
        else if (damageType == "Magic")
        {
            return damage - MagicResist;
        }
        else return damage;
    }

    public int GetArmor()
    {
        return Armor;
    }

    public int GetMagicResist()
    {
        return MagicResist;
    }
}
