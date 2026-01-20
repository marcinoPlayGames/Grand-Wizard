using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class EndLevel : MonoBehaviour
{
    public string sceneToLoad = "WinScene";  // Scena docelowa po zakończeniu poziomu
    public int levelToUnlock = 4;            // Ręcznie ustawiany numer levela do odblokowania

    public string levelID = "Level1";

    public int completedLevelToSet = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Jeśli levelToUnlock > aktualnie odblokowanego, zaktualizuj GameManager
            if (levelToUnlock > GameManager.Instance.GetUnlockedLevel())
            {
                GameManager.Instance.SetUnlockedLevel(levelToUnlock);
                StatSystem.Instance.SaveGameRuntime(); // zapis statystyk
            }

            // Przejdź do sceny zakończenia
            SceneManager.LoadScene(sceneToLoad);

            EndLevelAnalytics();
        }
    }

    void EndLevelAnalytics()
    {
        LevelAnalytics.Instance.tryNumber += 1;

        AnalyticsManager.Instance.PerformanceAnalytics(levelID);

        GameManager.Instance.SetLevelCompleted(completedLevelToSet);

        AnalyticsManager.Instance.LevelID = levelID;

        AnalyticsManager.Instance.EndsLevel(true, levelID);

        AnalyticsManager.Instance.PlayerDeath(levelID);

        AnalyticsManager.Instance.EnemyKilled(levelID);

        if (SceneManager.GetActiveScene().name == "Level5")
        {
            LevelAnalytics.Instance.boss_attempts += 1;
            AnalyticsManager.Instance.BossFightEnd(true);
        }

        AnalyticsManager.Instance.WeaponUsed(levelID);
    }

    
}