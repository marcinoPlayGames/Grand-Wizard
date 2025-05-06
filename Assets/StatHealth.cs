using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealth : MonoBehaviour
{
    public int Health;
    public int Healing_From_Damage_Percent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth()
    {
        return Health;
    }

    public void HealFromDamage(int damage)
    {
        Health = damage * Healing_From_Damage_Percent;
    }
}
