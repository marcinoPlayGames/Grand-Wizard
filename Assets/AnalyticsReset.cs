using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticsReset : MonoBehaviour
{
    [SerializeField]
    string sceneName = "LoreLevel";
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            GameManager.Instance.ResetAnalytics();
        }
    }
}
