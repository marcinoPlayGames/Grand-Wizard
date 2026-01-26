using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapGoBackButton : MonoBehaviour
{
    public string startMenuScene;

    public string nextLevelScene;
    public void LoadLevel()
    {
        int level = GameManager.Instance.GetUnlockedLevel();

        if (level != 8 && level != 9)
        {
            SceneManager.LoadScene(nextLevelScene);
        }
        else
        {
            SceneManager.LoadScene(startMenuScene);
        }
    }
}
