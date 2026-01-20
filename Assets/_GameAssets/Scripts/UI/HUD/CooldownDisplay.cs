using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownDisplay : MonoBehaviour
{
    public Image cooldownImage; // Przeci¹gnij tutaj Pasek_Cooldown
    private float cooldownTime = 5f;

    public void StartUIAttackCooldown(float cooldown)
    {
        cooldownTime = cooldown;

        StartCoroutine(AttackUICooldown());
    }

    public IEnumerator AttackUICooldown()
    {
        float t = 0f;

        while (t < cooldownTime)
        {
            t += Time.unscaledDeltaTime;
            cooldownImage.fillAmount = 1.0f - (t / cooldownTime);

            yield return null;
        }

        cooldownImage.fillAmount = 0f;
    }
}
