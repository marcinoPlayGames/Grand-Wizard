using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    public string levelToLoad = "SampleScene";

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelToLoad);
        Debug.Log("Level Loaded!");
    }
}