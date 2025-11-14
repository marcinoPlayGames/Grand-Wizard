using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class LoadLevelButton : MonoBehaviour
{
    public string levelToLoad = "SampleScene";

    public bool loadLevelManually = false;

    public bool unlockLevelOnClick = false;

    [ShowIf("unlockLevelOnClick")]
    public int levelToUnlock;
    public void LoadLevel()
    {
        int level = GameManager.Instance.GetUnlockedLevel();

        Debug.Log("[LoadLevelButton] level number loaded is: " + level);

        if (unlockLevelOnClick)
        {
            if (levelToUnlock > GameManager.Instance.GetUnlockedLevel())
            {
                GameManager.Instance.SetUnlockedLevel(levelToUnlock);
                StatSystem.Instance.SaveGame(); // zapis statystyk
            }
        }

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