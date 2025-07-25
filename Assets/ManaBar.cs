using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBar : MonoBehaviour
{
    public PlayerMove playerMove;
    
    public Image manaBarImage;
    private float maxMana;
    private float currentMana;
    private RectTransform barTransform;
    public TMP_Text Mana_Amount;

    // Optionally, define colors for different health states
    public Color fullManaColor;
    public Color lowManaColor;

    private float initialWidth;
    // Start is called before the first frame update
    void Start()
    {
        maxMana = playerMove.MaxMana;
        currentMana = playerMove.MaxMana;
        Debug.Log("Mana Bar Max Mana = " + maxMana);
        Debug.Log(manaBarImage.color);
        barTransform = GetComponent<RectTransform>();
        initialWidth = barTransform.rect.width;
        Mana_Amount.text = $"{currentMana}/{maxMana}";
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateHealthBar();
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

    public void UpdateManaBar()
    {
        currentMana = playerMove.GetMana();
        float manaPercent = currentMana / maxMana;

        Mana_Amount.text = $"{currentMana}/{maxMana}";

        barTransform.sizeDelta = new Vector2(manaPercent * initialWidth, barTransform.sizeDelta.y);

        manaBarImage.fillAmount = manaPercent;

        Debug.Log("Mana Fill amount = " + manaBarImage.fillAmount);
        manaBarImage.color = Color.Lerp(lowManaColor, fullManaColor, manaPercent);

        Debug.Log("Mana Fill color = " + manaBarImage.color);

        // Optionally, change the color based on health state
        //healthBarImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
    }
}
