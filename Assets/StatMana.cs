using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMana : MonoBehaviour
{
    public float Max_Mana;
    private float Mana;

    public float Mana_Regen;

    private Coroutine regenCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Max_Mana = GetStatValues("Mana");
        Mana = GetStatValues("Mana");
        Mana_Regen = GetStatValues("Mana_Regen");
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

    public void StartManaRegen()
    {
        Debug.Log("Mana Regen = " + Mana_Regen);
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenMana());
            Debug.Log("Regen started.");
        }
    }

    public float GetStatValues(string statName)
    {
        if (statName == "Mana")
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
        PlayerMove playerMove = GetComponent<PlayerMove>();

        while (true)
        {
            playerMove.RegenMana(Mana_Regen);

            if (playerMove.GetMana() >= playerMove.MaxMana)
            {
                playerMove.SetMaxMana();

                // Zatrzymaj coroutine prawidłowo
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
                yield break; // wyjście z pętli
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
