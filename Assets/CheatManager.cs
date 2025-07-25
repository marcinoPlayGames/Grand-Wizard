using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance;

    public PlayerMove playerMove;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            KillAllEvilKnights();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            KillPlayer();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void KillAllEvilKnights()
    {
        var enemies = GameObject.FindGameObjectsWithTag("NPC");
        int count = 0;

        foreach (var enemyObj in enemies)
        {
            var enemy = enemyObj.GetComponent<NPCController>();
            if (enemy != null)
            {
                enemy.DamageNPC(8888);
                count++;
            }
        }

        Debug.Log($"[CHEAT] Zabito {count} przeciwników typu EvilKnight");
    }

    void KillPlayer()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerMove = playerObj.GetComponent<PlayerMove>();
            Debug.Log("Found player!");
        }

        playerMove.DamagePlayer(88888, DamageType.True);
    }
}
