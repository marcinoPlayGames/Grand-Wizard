using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealth : MonoBehaviour
{
    public float Max_Health;
    private float Health;

    public float Healing_From_Damage_Percent;

    public float Health_Regen;
    // Start is called before the first frame update
    void Start()
    {
        Max_Health = GetStatValues("Max_Health");
        Health = GetStatValues("Health");
        Healing_From_Damage_Percent = GetStatValues("Healing_From_Damage_Percent");
        Health_Regen = GetStatValues("HP_Regen");

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

    public float GetStatValues(string statName)
    {
        if (statName == "Max_Health")
        {
            return Max_Health = StatSystem.Instance.GetStatValue("HP");
        }
        else if (statName == "Health")
        {
            return Health = StatSystem.Instance.GetStatValue("HP");
        }
        else if (statName == "Healing")
        {
            return Healing_From_Damage_Percent = StatSystem.Instance.GetStatValue("Healing");
        }
        else if (statName == "Health_Regen")
        {
            return Health_Regen = StatSystem.Instance.GetStatValue("HP_Regen");
        }
        else
        {
            return 0;
        }
    }

    public float GetHealth()
    {
        return Health;
    }

    public void HealFromDamage(float damage)
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
