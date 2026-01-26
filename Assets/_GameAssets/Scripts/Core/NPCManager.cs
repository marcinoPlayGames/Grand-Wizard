using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    public TMP_Text NPCLeft;
    public EndingCondition terrainBlock;

    private int totalEnemies;
    private int deadEnemies;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // np. przelicz NPC w tej scenie:
        RecalculateEnemies();
        FindBlockageAndDeathCounterText();
    }

    void FindBlockageAndDeathCounterText()
    {
        GameObject terrainObj = GameObject.Find("Blokada");
        if (terrainObj != null)
            terrainBlock = terrainObj.GetComponent<EndingCondition>();

        GameObject npcText = GameObject.Find("NPCDeathCounter");
        if (npcText != null)
            NPCLeft = npcText.GetComponent<TMP_Text>();
    }

    void RecalculateEnemies()
    {
        ResetEnemies();
        
        totalEnemies = GameObject.FindGameObjectsWithTag("NPC").Length;
    }

    public void EnemyDied()
    {
        deadEnemies++;
        UpdateUI();

        if (deadEnemies >= totalEnemies)
            UnlockTerrain();
    }

    void UpdateUI()
    {
        if (NPCLeft != null)
            NPCLeft.text = $"{totalEnemies - deadEnemies}";
    }

    void UnlockTerrain()
    {
        if (terrainBlock != null)
            terrainBlock.HideTerrain();
    }

    void ResetEnemies()
    {
        totalEnemies = 0;
        deadEnemies = 0;
    }
}
