using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticals : MonoBehaviour
{
    public int Attack_Critical_Hit_Chance;
    public int Attack_Critical_Hit_Damage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsCriticalHit()
    {
        int critChance = Attack_Critical_Hit_Chance;
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < critChance;
    }

    public void GetCriticalDamageByDamageType(int damage, int damageType)
    {
        if (damageType == "Physical")
        {
            return damage * Attack_Critical_Hit_Damage;
        }
    }
}
