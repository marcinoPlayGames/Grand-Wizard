using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class TestVSync : MonoBehaviour
{
    public int frameRate = 30;

    [Button]
    void TestSync()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate; // albo 20
    }
}
