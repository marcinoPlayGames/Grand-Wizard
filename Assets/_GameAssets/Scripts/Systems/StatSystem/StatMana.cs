using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StatMana : MonoBehaviour
{
    public float Max_Mana;
    private float Mana;

    public float Mana_Regen;

    private Coroutine regenCoroutine;

    [SerializeField]
    PlayerMove playerMove;

    private static readonly WaitForSeconds oneSecond = new WaitForSeconds(1f);
    void Awake()
    {
        Max_Mana = GetStatValues("Mana");
        Mana = GetStatValues("Mana");
        Mana_Regen = GetStatValues("Mana_Regen");
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => StatSystem.Instance != null && StatSystem.Instance.IsReady);

        StartCoroutine(RegenMana());
    }

    public void StartManaRegen()
    {
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenMana());
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
        yield return new WaitForSeconds(4f);

        if (playerMove.GetMana() >= playerMove.MaxMana)
        {
            playerMove.SetMaxMana();

            // Zatrzymaj coroutine prawidłowo
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }

            regenCoroutine = null;
            yield break; // wyjście z pętli
        }

        while (true)
        {
            playerMove.RegenMana(Mana_Regen);

            if (playerMove.GetMana() >= playerMove.MaxMana)
            {
                playerMove.SetMaxMana();

                // Zatrzymaj coroutine prawidłowo
                if (regenCoroutine != null)
                {
                    StopCoroutine(regenCoroutine);
                }
                
                regenCoroutine = null;
                yield break; // wyjście z pętli
            }

            yield return oneSecond;
        }
    }
}
