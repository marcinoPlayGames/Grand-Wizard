using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static bool cutscenePlaying;

    [SerializeField] 
    private string nextSceneName;
    [SerializeField]
    private int levelToUnlock;
    [SerializeField]
    PlayableDirector director;

    bool skipUsed = false;

    [SerializeField]
    public Image blackScreen;
    [SerializeField]
    public float fadeTime = 0.25f;

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

    public void SkipCutscene()
    {
        if (skipUsed) return;

        skipUsed = true;

        director.Stop();
        StartCoroutine(FadeAndLoad());
    }

    public IEnumerator FadeAndLoad()
    {
        float t = 0f;

        blackScreen.gameObject.SetActive(true);

        Color c = blackScreen.color;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeTime);
            blackScreen.color = c;

            yield return null;
        }

        c.a = 1f;
        blackScreen.color = c;

        EndCutscene();
    }
}
