using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public string sceneToLoad = "WinScene";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextLevel > GameManager.Instance.GetUnlockedLevel())
            {
                GameManager.Instance.SetUnlockedLevel(nextLevel);
                StatSystem.Instance.SaveGame();
            }

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
