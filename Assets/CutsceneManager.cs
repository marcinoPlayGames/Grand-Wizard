using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static bool cutscenePlaying;

    [SerializeField] 
    private string nextSceneName;
    [SerializeField]
    private int levelToUnlock;

    public void StartCutscene()
    {
        cutscenePlaying = true;
    }

    public void EndCutscene()
    {
        cutscenePlaying = false;

        if (levelToUnlock > GameManager.Instance.GetUnlockedLevel())
        {
            GameManager.Instance.SetUnlockedLevel(levelToUnlock);
            StatSystem.Instance.SaveGameRuntime(); // zapis statystyk
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
