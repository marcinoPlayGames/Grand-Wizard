using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrapper : MonoBehaviour
{
    void Awake()
    {
        if (GameManager.Instance == null || StatSystem.Instance == null)
        {
            Debug.LogWarning("Nie znaleziono GameManager lub StatSystem. Ładowanie StartMenu...");
            SceneManager.LoadScene("StartMenu");
        }
    }
}