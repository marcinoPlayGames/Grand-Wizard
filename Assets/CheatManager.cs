using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance;

    public PlayerMove playerMove;

    public EndLevel endLevel;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            KillAllEvilKnights();
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            KillPlayer();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GetCoins();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CompleteLevel();
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

    void GetCoins()
    {
        GameManager.Instance.DebugAddCoins();
    }

    void CompleteLevel()
    {
        GameObject trophyObj = GameObject.Find("Trophy");
        if (trophyObj != null)
        {
            endLevel = trophyObj.GetComponent<EndLevel>();
            Debug.Log("Found trophy and level end!");
        }

        if (endLevel.levelToUnlock > GameManager.Instance.GetUnlockedLevel())
        {
            GameManager.Instance.SetUnlockedLevel(endLevel.levelToUnlock);
            Debug.Log("levelToUnlock = " + endLevel.levelToUnlock);
            StatSystem.Instance.SaveGame(); // zapis statystyk
        }

        SceneManager.LoadScene(endLevel.sceneToLoad);
    }
}
