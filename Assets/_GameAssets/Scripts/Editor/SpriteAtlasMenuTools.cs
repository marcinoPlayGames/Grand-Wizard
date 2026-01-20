#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.IO;

public class SpriteAtlasMenuTools
{
    [MenuItem("Tools/List Sprites In Atlas")]
    public static void ListSprites()
    {
        var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>("Assets/Resources/StatIcons/SA_StatIcons.spriteatlasv2");
        if (atlas == null)
        {
            Debug.LogError("Nie znaleziono atlasu");
            return;
        }

        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        foreach (var sprite in sprites)
        {
            Debug.Log("Sprite name: " + sprite.name);
            // sprite.texture to tekstura atlasu
            // sprite.rect to prostokąt na atlasie
        }
    }

    [MenuItem("Tools/Export SpriteAtlas Texture as PNG")]
    public static void ExportAtlasTexture()
    {
        // Wczytaj atlas
        var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>("Assets/Resources/StatIcons/SA_StatIcons.spriteatlasv2");
        if (atlas == null)
        {
            Debug.LogError("Nie znaleziono atlasu");
            return;
        }

        // Pobierz sprity z atlasu
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        if (sprites.Length == 0)
        {
            Debug.LogError("Atlas jest pusty");
            return;
        }

        // Weź teksturę pierwszego sprite'a (jest to tekstura całego atlasu)
        Texture2D atlasTexture = sprites[0].texture;

        if (atlasTexture == null)
        {
            Debug.LogError("Brak tekstury atlasu");
            return;
        }

        // UWAGA: atlasTexture może nie być "czytelny" (readable = false),
        // więc trzeba stworzyć kopię tekstury, która jest czytelna.

        Texture2D readableTex = GetReadableTexture(atlasTexture);

        // Zapisz do pliku PNG
        byte[] pngData = readableTex.EncodeToPNG();

        string path = "Assets/ExportedAtlas.png";
        File.WriteAllBytes(path, pngData);

        AssetDatabase.Refresh();

        Debug.Log("Atlas zapisany jako PNG: " + path);
    }

    private static Texture2D GetReadableTexture(Texture2D source)
    {
        // RenderTexture do kopiowania tekstury w trybie czytelnym
        RenderTexture tmp = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(source.width, source.height);
        readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }
}
#endif