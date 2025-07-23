using UnityEngine;
using UnityEngine.SceneManagement;

public class DevSceneSelector : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}