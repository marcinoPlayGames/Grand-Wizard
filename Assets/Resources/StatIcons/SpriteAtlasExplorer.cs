using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;   // konieczne do SpriteAtlas
using System.IO;

public class SpriteAtlasExporter
{
    /*[MenuItem("Tools/Export Sprite Atlas as PNG")]
    public static void ExportAtlas()
    {
        // Załaduj SpriteAtlas, a nie Texture2D
        var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>("Assets/Resources/SA_StatIcons.spriteatlasv2"); // poprawna ścieżka do SpriteAtlas

        if (atlas == null)
        {
            Debug.LogError("Nie znaleziono SpriteAtlas");
            return;
        }

        // Pobierz teksturę atlasu
        Texture2D atlasTexture = atlas.texture;

        if (atlasTexture == null)
        {
            Debug.LogError("SpriteAtlas ma null texture");
            return;
        }

        // Utwórz czytelną kopię tekstury
        RenderTexture tmp = RenderTexture.GetTemporary(atlasTexture.width, atlasTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(atlasTexture, tmp);

        RenderTexture.active = tmp;
        Texture2D readableTex = new Texture2D(atlasTexture.width, atlasTexture.height);
        readableTex.ReadPixels(new Rect(0, 0, atlasTexture.width, atlasTexture.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tmp);

        // Zapisz do PNG
        byte[] pngData = readableTex.EncodeToPNG();
        string path = "Assets/Exported_Atlas.png";
        File.WriteAllBytes(path, pngData);

        AssetDatabase.Refresh();
        Debug.Log("Atlas zapisany jako PNG w: " + path);
    }*/
}