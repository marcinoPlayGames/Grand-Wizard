using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMana : MonoBehaviour
{
    public int Max_Mana;
    private int Mana;

    public int Mana_Regen;
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

    public int GetMana()
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
