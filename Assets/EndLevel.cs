using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public string sceneToLoad = "WinScene";  // Scena docelowa po zakończeniu poziomu
    public int levelToUnlock = 4;            // Ręcznie ustawiany numer levela do odblokowania

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Jeśli levelToUnlock > aktualnie odblokowanego, zaktualizuj GameManager
            if (levelToUnlock > GameManager.Instance.GetUnlockedLevel())
            {
                GameManager.Instance.SetUnlockedLevel(levelToUnlock);
                StatSystem.Instance.SaveGameRuntime(); // zapis statystyk
            }

            // Przejdź do sceny zakończenia
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}