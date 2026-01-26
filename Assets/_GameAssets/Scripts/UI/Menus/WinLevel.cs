using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLevel : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene("StartMenu");
        Debug.Log("Level Loaded!");
    }
}
