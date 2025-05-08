using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealth : MonoBehaviour
{
    public int Max_Health;
    private int Health;

    public int Healing_From_Damage_Percent;

    public int Health_Regen;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RegenHealth());
    }

    // Update is called once per frame
    void Update()
    {
        if (Max_Health == Health)
        {
            StopCoroutine(RegenHealth());
        }
    }

    public int GetHealth()
    {
        return Health;
    }

    public void HealFromDamage(int damage)
    {
        Health += damage * Healing_From_Damage_Percent;
    }

    IEnumerator RegenHealth()
    {
        while (true)
        {
            Health += Health_Regen;
            yield return new WaitForSeconds(1f);
        }
    }
}
