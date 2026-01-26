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
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F3))
        {
            GetCoins();
        }
        if (Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F4))
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

#if UNITY_EDITOR
        Debug.Log($"[CHEAT] Zabito {count} przeciwników typu EvilKnight");
#endif
    }

    void KillPlayer()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerMove = playerObj.GetComponent<PlayerMove>();
#if UNITY_EDITOR
            Debug.Log("Found player!");
#endif
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
#if UNITY_EDITOR
            Debug.Log("Found trophy and level end!");
#endif
        }

        if (endLevel == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Trophy not found!");
#endif
        }

        if (endLevel.levelToUnlock > GameManager.Instance.GetUnlockedLevel())
        {
            GameManager.Instance.SetUnlockedLevel(endLevel.levelToUnlock);
#if UNITY_EDITOR
            Debug.Log("levelToUnlock = " + endLevel.levelToUnlock);
#endif
            StatSystem.Instance.SaveGameRuntime(); // zapis statystyk
        }

        SceneManager.LoadScene(endLevel.sceneToLoad);
    }
}
