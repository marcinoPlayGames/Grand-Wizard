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

    [SerializeField]
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
        initialWidth = barTransform.rect.width;
        Mana_Amount.text = $"{currentMana}/{maxMana}";
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

        manaBarImage.color = Color.Lerp(lowManaColor, fullManaColor, manaPercent);

        // Optionally, change the color based on health state
        //healthBarImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
    }
}
