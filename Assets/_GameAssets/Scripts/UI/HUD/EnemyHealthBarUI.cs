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

    public void Bind(NPCController controller)
    {
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
        fill.fillAmount = hp / maxHp;
        hpText.text = $"{hp}/{maxHp}";
        visibleTimer = 3f;
        gameObject.SetActive(true);
    }

    public void ResetUI()
    {
        if (npc != null)
            npc.OnNPCHealthChange -= Refresh;

        npc = null;
        visibleTimer = 0f;
    }
}