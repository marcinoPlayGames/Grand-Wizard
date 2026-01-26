using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealth : MonoBehaviour
{
    public float Max_Health;
    private float Health;

    public float Healing_From_Damage_Percent;

    public float Health_Regen;

    private bool bIsRegenStarted = false;

    private Coroutine regenCoroutine;

    private int increasedRegenCount = 0;

    [SerializeField]
    PlayerMove playerMove;

    void Awake()
    {
        Max_Health = GetStatValues("Max_Health");
        Health = GetStatValues("Health");
        Healing_From_Damage_Percent = GetStatValues("Healing");
        Health_Regen = GetStatValues("HP_Regen");

        UnityEngine.Debug.Log($"[StatHealth] Max_Health is {Max_Health}");
        UnityEngine.Debug.Log($"[StatHealth] Health is {Health}");
        UnityEngine.Debug.Log($"[StatHealth] Healing is {Healing_From_Damage_Percent}");
        UnityEngine.Debug.Log($"[StatHealth] Health_Regen is {Health_Regen}");
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => StatSystem.Instance != null && StatSystem.Instance.IsReady);
    }

    public void StartHealthRegen()
    {
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenHealth());
            Debug.Log("Regen started.");
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
        else if (statName == "HP_Regen")
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
        Debug.Log("Healing value = " + Healing_From_Damage_Percent);
        playerMove.HealPlayer(Mathf.CeilToInt(damage * (Healing_From_Damage_Percent / 100)));
    }

    public float GetHealingValue(float damage)
    {
        return Mathf.CeilToInt(damage = damage * (Healing_From_Damage_Percent / 100));
    }

    IEnumerator RegenHealth()
    {
        while (true)
        {
            playerMove.HealPlayer(Health_Regen * (playerMove.increasedRegen_HP ? 2 : 1));

            if (playerMove.increasedRegen_HP)
            {
                increasedRegenCount++;

                if (increasedRegenCount >= 5)
                {
                    playerMove.increasedRegen_HP = false;
                }
            }

            if (playerMove.GetHealth() >= playerMove.Player_MaxHealth)
            {
                playerMove.SetMaxHealth();
                bIsRegenStarted = false;

                // Zatrzymaj coroutine prawidłowo
                if (regenCoroutine != null)
                {
                    StopCoroutine(regenCoroutine);
                }
                
                regenCoroutine = null;
                yield break; // wyjście z pętli
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
