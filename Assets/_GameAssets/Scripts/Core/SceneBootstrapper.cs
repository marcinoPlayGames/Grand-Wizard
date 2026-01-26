using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrapper : MonoBehaviour
{
    void Awake()
    {
        if (GameManager.Instance == null || StatSystem.Instance == null)
        {
            SceneManager.LoadScene("StartMenu");
        }
    }
}