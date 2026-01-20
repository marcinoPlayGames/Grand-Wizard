using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class LoadLevelButton : MonoBehaviour
{
    public string levelToLoad = "SampleScene";

    public bool loadLevelManually = false;

    public bool unlockLevelOnClick = false;

    public bool saveAnalyticsDecisionOnClick = false;

    public bool loadOtherLevelIfAnalyticsAgree = false;

    [ShowIf("loadOtherLevelIfAnalyticsAgree")]
    public string otherLevelToLoad = "SampleScene";

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
                StatSystem.Instance.SaveGameRuntime(); // zapis statystyk
            }
        }

        if (!loadLevelManually)
        {
            SceneManager.LoadScene(level);
        }
        else
        {
            if (loadOtherLevelIfAnalyticsAgree)
            {
                if (PlayerPrefs.GetFloat("AnalyticsAgree") == 1) SceneManager.LoadScene(otherLevelToLoad);
                if (PlayerPrefs.GetFloat("AnalyticsAgree") != 1) SceneManager.LoadScene(levelToLoad);


            }
            else
            {
                SceneManager.LoadScene(levelToLoad);
            }  
        }

        if (saveAnalyticsDecisionOnClick) PlayerPrefs.SetFloat("AnalyticsAgree", 1);

        Debug.Log("Level Loaded!");
    }
}