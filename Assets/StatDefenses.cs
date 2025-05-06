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

    public void GetDamageByDamageType(int damage, int damageType)
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

    public void GetArmor()
    {
        return Armor;
    }

    public void GetMagicResist()
    {
        return MagicResist;
    }
}
