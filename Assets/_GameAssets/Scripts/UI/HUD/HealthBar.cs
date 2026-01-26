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

    [SerializeField]
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
        initialWidth = barTransform.rect.width;
        HP_Amount.text = $"{currentHealth}/{maxHealth}";
    }

    void Awake()
    {
        if (playerMove == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                playerMove = playerObj.GetComponent<PlayerMove>();
        }
    }

    public void UpdateHealthBar()
    {
        currentHealth = playerMove.GetHealth();
        float healthPercent = currentHealth / maxHealth;

        HP_Amount.text = $"{currentHealth}/{maxHealth}";

        Image s;

        s = GetComponent<Image>();

        // Set the size of the health bar
        s.fillAmount = healthPercent;

        barTransform.sizeDelta = new Vector2(healthPercent * initialWidth, barTransform.sizeDelta.y);

        // Optionally, change the color based on health state
        //healthBarImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        s.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
    }
}
