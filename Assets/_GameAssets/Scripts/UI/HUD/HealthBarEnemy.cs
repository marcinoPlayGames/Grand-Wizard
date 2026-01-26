using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarEnemy : MonoBehaviour
{
    public Image healthBarImage;
    private float maxHealth;
    private float currentHealth;
    
    public TMP_Text HP_Amount;

    // Optionally, define colors for different health states
    public Color fullHealthColor;
    public Color lowHealthColor;

    private float initialWidth;

    public bool showHealthBarOnlyOnDamage = false;
    public bool showHealthTriggered = false;

    public GameObject healthBarObject;

    private Coroutine hideCoroutine;

    [SerializeField]
    NPCController npcController;

    [SerializeField]
    private RectTransform barTransform;

    void Start()
    {   
        maxHealth = npcController.NPC_MaxHealth;
        currentHealth = npcController.NPC_MaxHealth;
        Debug.Log(maxHealth);
        Debug.Log(healthBarImage.color);
        initialWidth = barTransform.rect.width;
        HP_Amount.text = $"{currentHealth}/{maxHealth}";

        healthBarObject.SetActive(true);

        if (showHealthBarOnlyOnDamage || showHealthTriggered)
        {
            healthBarObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        npcController.OnNPCHealthChange += UpdateHealthBar;
    }
    private void OnDisable()
    {
        npcController.OnNPCHealthChange -= UpdateHealthBar;
    }

    public void UpdateHealthBar()
    {
        if (showHealthBarOnlyOnDamage)
        {
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);

            hideCoroutine = StartCoroutine(HideHealthBar());
        }
        
        currentHealth = npcController.GetNPCHealth();
        float healthPercent = currentHealth / maxHealth;

        HP_Amount.text = $"{currentHealth}/{maxHealth}";

        Image s;

        s = healthBarImage;

        Debug.Log($"[HealthBarEnemy] currentHealth = {currentHealth}");
        Debug.Log($"[HealthBarEnemy] maxHealth = {maxHealth}");
        Debug.Log($"[HealthBarEnemy] healthPercent = {healthPercent}");

        Debug.Log(s.color);

        // Set the size of the health bar
        s.fillAmount = healthPercent;

        Debug.Log("Health Fill Amount = " + s.fillAmount);
        Debug.Log($"[HealthBarEnemy] initialWidth = {initialWidth}");
        Debug.Log($"[HealthBarEnemy] percent = {initialWidth * healthPercent}");

        barTransform.sizeDelta = new Vector2(healthPercent * initialWidth, barTransform.sizeDelta.y);

        // Optionally, change the color based on health state
        //healthBarImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        s.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
    }

    IEnumerator HideHealthBar()
    {
        healthBarObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        healthBarObject.SetActive(false);
        hideCoroutine = null;
    }
}
