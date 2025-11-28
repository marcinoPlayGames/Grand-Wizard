using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Services.Core;
using UnityEngine;
using System;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    static readonly ProfilerMarker Marker = new ProfilerMarker(name: "Markiiiiing");

    public static AnalyticsManager Instance { get; private set; }

#if USE_ANALYTICS

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

    async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("[Analytics] Initialized");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
#endif
    }

#if USE_ANALYTICS
    public void SendEvent(string eventName)
    {
        AnalyticsService.Instance.RecordEvent(eventName);
    }

    public void SendEvent(string eventName, Dictionary<string, object> parameters)
    {
        try
        {
            var ev = new CustomEvent(eventName);
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    // dopuszczalne typy: string, int, long, float, double, bool
                    ev[kv.Key] = kv.Value;
                }
            }
            AnalyticsService.Instance.RecordEvent(ev);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Analytics] Failed to send event '{eventName}': {e.Message}");
        }
    }
#endif
}
