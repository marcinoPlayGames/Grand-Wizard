using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMana : MonoBehaviour
{
    public float Max_Mana;
    private float Mana;

    public float Mana_Regen;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RegenMana());
    }

    // Update is called once per frame
    void Update()
    {
        if (Max_Mana == Mana)
        {
            StopCoroutine(RegenMana());
        }
    }

    public float GetStatValues(string statName)
    {
        if (statName == "Max_Mana")
        {
            return Max_Mana = StatSystem.Instance.GetStatValue(statName);
        }
        else if (statName == "Mana_Regen")
        {
            return Mana_Regen = StatSystem.Instance.GetStatValue(statName);
        }
        else
        {
            return 0;
        }
    }

    public float GetMana()
    {
        return Mana;
    }

    IEnumerator RegenMana()
    {
        while (true)
        {
            Mana += Mana_Regen;
            yield return new WaitForSeconds(1f);
        }
    }
}
