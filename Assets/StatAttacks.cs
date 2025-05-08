using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAttacks : MonoBehaviour
{
    public int Attack_Damage;
    public int Magic_Damage;
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
}
