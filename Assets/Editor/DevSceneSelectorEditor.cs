#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public static class DevSceneSelectorEditor
{
    [MenuItem("Tools/Select Scene/Generate Menu %#g")] // Ctrl+Shift+G
    public static void GenerateSceneMenu()
    {
        // Sprawdź, czy na scenie istnieje DevSceneSelector
        if (Object.FindObjectOfType<DevSceneSelector>() == null)
        {
            EditorUtility.DisplayDialog("Brak DevSceneSelector", "Nie znaleziono DevSceneSelector w aktualnej scenie!", "OK");
            return;
        }

        string[] scenes = GetSceneNames();

        foreach (string scene in scenes)
        {
            Debug.Log($"Dodana scena: {scene}");
        }

        EditorUtility.DisplayDialog("Sceny znalezione", "Sceny zostały znalezione w konsoli. Dodaj dla nich ręcznie MenuItem-y lub zrób dynamiczny edytor.", "OK");
    }

    private static string[] GetSceneNames()
    {
        int sceneCount = EditorBuildSettings.scenes.Length;
        string[] names = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            string path = EditorBuildSettings.scenes[i].path;
            names[i] = Path.GetFileNameWithoutExtension(path);
        }

        return names;
    }

    // Możesz dodać statycznie wpisy – lub zrobić wersję generującą dynamicznie
    [MenuItem("Tools/Select Scene/Level1")]
    public static void LoadLevel1()
    {
        OpenSceneByName("Level1");
    }

    [MenuItem("Tools/Select Scene/UpgradeSystem")]
    public static void LoadUpgradeSystem()
    {
        OpenSceneByName("UpgradeSystem");
    }

    [MenuItem("Tools/Select Scene/StatisticTree")]
    public static void LoadStatisticTree()
    {
        OpenSceneByName("StatisticTree");
    }

    [MenuItem("Tools/Select Scene/Level1_Demo")]
    public static void LoadLevel1Demo()
    {
        OpenSceneByName("Level1_Demo");
    }

    [MenuItem("Tools/Select Scene/Level2_Demo")]
    public static void LoadLevel2Demo()
    {
        OpenSceneByName("Level2_Demo");
    }

    [MenuItem("Tools/Select Scene/Level3")]
    public static void LoadLevel3()
    {
        OpenSceneByName("Level3");
    }

    [MenuItem("Tools/Select Scene/Level4")]
    public static void LoadLevel4()
    {
        OpenSceneByName("Level4");
    }

    [MenuItem("Tools/Select Scene/Level5")]
    public static void LoadLevel5()
    {
        OpenSceneByName("Level5");
    }

    private static void OpenSceneByName(string sceneName)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.path.Contains(sceneName))
            {
                if (Application.isPlaying)
                {
                    // Jesteśmy w Play Mode → użyj SceneManager
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    // Jesteśmy w edytorze → użyj EditorSceneManager
                    EditorSceneManager.OpenScene(scene.path);
                }
                return;
            }
        }

        EditorUtility.DisplayDialog("Scena nie znaleziona", $"Nie znaleziono sceny: {sceneName} w Build Settings.", "OK");
    }
}
#endif