using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    public string levelToLoad = "SampleScene";

    public bool loadLevelManually = false;

    public void LoadLevel()
    {
        int level = GameManager.Instance.GetUnlockedLevel();

        Debug.Log(level);

        if (!loadLevelManually)
        {
            SceneManager.LoadScene(level);
        }
        else
        {
            SceneManager.LoadScene(levelToLoad);
        }

        Debug.Log("Level Loaded!");
    }
}