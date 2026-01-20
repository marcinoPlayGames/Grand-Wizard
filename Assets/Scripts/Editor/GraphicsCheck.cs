using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsCheck : MonoBehaviour
{
    [MenuItem("Tools/SprawdŸ Render Pipeline")]
    public static void CheckPipeline()
    {
        if (GraphicsSettings.currentRenderPipeline != null)
        {
            string pipelineName = GraphicsSettings.currentRenderPipeline.GetType().ToString();

            if (pipelineName.Contains("Universal"))
                Debug.Log("<b>Twój projekt u¿ywa:</b> <color=cyan>URP (Universal Render Pipeline)</color>");
            else if (pipelineName.Contains("HighDefinition"))
                Debug.Log("<b>Twój projekt u¿ywa:</b> <color=yellow>HDRP (High Definition Render Pipeline)</color>");
            else
                Debug.Log("<b>U¿ywasz niestandardowego SRP:</b> " + pipelineName);
        }
        else
        {
            Debug.Log("<b>Twój projekt u¿ywa:</b> <color=white>Built-in Render Pipeline</color>");
        }
    }
}
