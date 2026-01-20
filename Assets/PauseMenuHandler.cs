using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField]
    GameObject canvasObject;

    [SerializeField]
    string startMenuLevel;

    bool pauseOpened = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseOpened) PauseGame();
            else ContinueGame();

            pauseOpened = !pauseOpened;
        }
    }

    public void PauseGame()
    {
        canvasObject.SetActive(true);

        Time.timeScale = 0f;

        GameManager.Instance.gamePaused = true;
    }

    public void ContinueGame()
    {
        canvasObject.SetActive(false);

        Time.timeScale = 1f;

        GameManager.Instance.gamePaused = false;
    }

    public void RestartLevel()
    {
        int level = GameManager.Instance.GetUnlockedLevel();

        SceneManager.LoadScene(level);

        Time.timeScale = 1f;

        GameManager.Instance.gamePaused = false;
    }

    public void ExitLevel()
    {
        SceneManager.LoadScene(startMenuLevel);

        Time.timeScale = 1f;

        GameManager.Instance.gamePaused = false;
    }
}
