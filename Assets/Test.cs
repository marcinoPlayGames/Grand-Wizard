using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Services.Core;
using UnityEngine;
using System;
using Unity.Services.Analytics;

public class Test : MonoBehaviour
{
    static readonly ProfilerMarker Marker = new ProfilerMarker(name: "Markiiiiing");

#if USE_ANALYTICS

    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Start()
    {
        Debug.Log("X");

        AnalyticsService.Instance.StartDataCollection();
    }

    void Update()
    {
        if (Input.GetKeyDown(name: "space"))
        {
            Debug.Log("Pressed space!");
            AnalyticsService.Instance.RecordEvent(eventName: "keyPressed");
        }
    }

#endif
}
