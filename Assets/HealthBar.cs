using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public PlayerMove playerMove;
    
    public Image healthBarImage;
    private float maxHealth;
    private float currentHealth;
    private RectTransform barTransform;
    public TMP_Text HP_Amount;

    // Optionally, define colors for different health states
    public Color fullHealthColor;
    public Color lowHealthColor;

    private float initialWidth;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = playerMove.Player_MaxHealth;
        currentHealth = playerMove.Player_MaxHealth;
        Debug.Log(maxHealth);
        Debug.Log(healthBarImage.color);
        barTransform = GetComponent<RectTransform>();
        initialWidth = barTransform.rect.width;
        HP_Amount.text = $"{currentHealth}/{maxHealth}";
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        currentHealth = playerMove.GetHealth();
        float healthPercent = currentHealth / maxHealth;

        HP_Amount.text = $"{currentHealth}/{maxHealth}";

        Image s;

        s = GetComponent<Image>();

        Debug.Log(s.color);

        // Set the size of the health bar
        s.fillAmount = healthPercent;

        Debug.Log(s.fillAmount);

        barTransform.sizeDelta = new Vector2(healthPercent * initialWidth, barTransform.sizeDelta.y);

        // Optionally, change the color based on health state
        //healthBarImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        s.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
    }
}
