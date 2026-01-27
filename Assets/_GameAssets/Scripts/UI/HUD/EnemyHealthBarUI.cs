using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBarUI : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] TMP_Text hpText;

    private NPCController npc;
    private float maxHp;
    private float visibleTimer;

    [SerializeField]
    private RectTransform barTransform;

    private float initialWidth;

    public Color fullHealthColor;
    public Color lowHealthColor;
    public void Bind(NPCController controller)
    {
        initialWidth = barTransform.rect.width;

        npc = controller;
        maxHp = npc.NPC_MaxHealth;
        npc.OnNPCHealthChange += Refresh;
        Refresh();      
    }

    void LateUpdate()
    {
        if (!npc) return;

        transform.position = npc.transform.position + Vector3.up * 1.5f;

        visibleTimer -= Time.deltaTime;
        if (visibleTimer <= 0)
            gameObject.SetActive(false);
    }

    void Refresh()
    {
        float hp = npc.GetNPCHealth();

        maxHp = npc.NPC_MaxHealth;

        float healthPercent = hp / maxHp;

        //fill.fillAmount = healthPercent;

        hpText.text = $"{hp}/{maxHp}";
        visibleTimer = 3f;
        gameObject.SetActive(true);

        Debug.Log($"hp = {hp}, maxHp = {maxHp}, healthPercent = {healthPercent}, initialWidth = {initialWidth}, size should be = {healthPercent * initialWidth}");

        barTransform.sizeDelta = new Vector2(healthPercent * initialWidth, barTransform.sizeDelta.y);

        fill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
    }

    public void ResetUI()
    {
        if (npc != null)
            npc.OnNPCHealthChange -= Refresh;

        npc = null;
        visibleTimer = 0f;
    }
}