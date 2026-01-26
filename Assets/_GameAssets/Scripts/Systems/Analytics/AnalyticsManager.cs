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

    [SerializeField]
    string playerType;
    public static AnalyticsManager Instance { get; private set; }

    public string LevelID = string.Empty;

#if USE_ANALYTICS

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
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogException(e);
#endif
        }
#endif
    }

    float fpsSum = 0f;
    int fpsSamples = 0;

    float fpsMin = 0f;

    void Update()
    {
#if USE_ANALYTICS
        float dt = Time.unscaledDeltaTime;
        if (dt <= 0f) return;

        float fps = 1f / dt;

        if (fpsSamples == 0)
            fpsMin = fps;
        else
            fpsMin = Mathf.Min(fpsMin, fps);

        fpsSum += fps;
        fpsSamples++;
#endif
    }

#if USE_ANALYTICS
    public void SendEvent(string eventName)
    {
        AnalyticsService.Instance.RecordEvent(eventName);
    }

    public void SendEvent(string eventName, Dictionary<string, object> parameters)
    {
        if (Instance == null)
        {
#if UNITY_EDITOR
            Debug.LogError("AnalyticsManager not initialized");
#endif
            return;
        }

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
#if UNITY_EDITOR
            Debug.LogWarning($"[Analytics] Failed to send event '{eventName}': {e.Message}");
#endif
        }
    }

    private void OnApplicationQuit()
    {
        EndSession();
        EndsLevel(false, LevelID);
        PlayerDeath(LevelID);
        EnemyKilled(LevelID);
        BossFightEnd(false);
        WeaponUsed(LevelID);
        UpgradeBoughtTotal(LevelID);
    }

    void EndSession()
    {
        float avgFps = fpsSamples > 0 ? fpsSum / fpsSamples : 0f;

        avgFps = (float)Math.Round(avgFps, 2);

        var data = new Dictionary<string, object>
        {
            { AnalyticsParams.Duration, Time.time },
            { AnalyticsParams.LevelsCompleted, GameManager.Instance.levelsCompleted },
            { AnalyticsParams.CoinsTotal, GameManager.Instance.coinsCollected },
            { AnalyticsParams.DiamondsTotal, GameManager.Instance.diamondsCollected },
            { AnalyticsParams.TreasuresTotal, GameManager.Instance.treasuresCollected },
            { AnalyticsParams.FPSAvg, avgFps },
            { AnalyticsParams.PlayerType, playerType }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.SessionEnd, data);
    }

    public void PerformanceAnalytics(string levelID)
    {
        float fpsAvg = fpsSamples > 0 ? fpsSum / fpsSamples : 0f;

        var data = new Dictionary<string, object>
        {
            { AnalyticsParams.LevelId, levelID },
            { AnalyticsParams.FPSAvg, fpsAvg },
            { AnalyticsParams.FPSMin, fpsMin },
            { AnalyticsParams.DeviceModel, SystemInfo.deviceModel },
            { AnalyticsParams.Resolution, Screen.currentResolution.width + "x" + Screen.currentResolution.height }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.Performance, data);
    }

    public void EndsLevel(bool result, string levelID)
    {
        var data = new Dictionary<string, object>
        {
            { AnalyticsParams.LevelId, levelID },
            { AnalyticsParams.TryNumber, LevelAnalytics.Instance.tryNumber },
            { AnalyticsParams.Duration, LevelAnalytics.Instance.duration },
            { AnalyticsParams.Result, result },
            { AnalyticsParams.Deaths, LevelAnalytics.Instance.deaths },
            { AnalyticsParams.DamageTaken, LevelAnalytics.Instance.damageTaken },
            { AnalyticsParams.DamageDone, LevelAnalytics.Instance.damageDone },
            { AnalyticsParams.CoinsTotal, LevelAnalytics.Instance.coinsTotal },
            { AnalyticsParams.DiamondsTotal, LevelAnalytics.Instance.diamondsTotal },
            { AnalyticsParams.TreasuresTotal, LevelAnalytics.Instance.treasuresTotal },
            { AnalyticsParams.HPLeft, LevelAnalytics.Instance.hpLeft },
            { AnalyticsParams.UsedUpgrades, WeaponUpgradeSystem.Instance.GetLevel("Staff") }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.LevelEnd, data);
    }

    public void PlayerDeath(string levelID)
    {
        for (int i = 0; i < LevelAnalytics.Instance.causeDeathsNumber.Count; i++)
        {
            var cause = LevelAnalytics.Instance.causeDeathsNumber[i].cause;
            var deathsCount = LevelAnalytics.Instance.causeDeathsNumber[i].count;

            var data = new Dictionary<string, object>
            {
                { AnalyticsParams.LevelId, levelID },
                { AnalyticsParams.Cause, cause },
                { AnalyticsParams.posX, LevelAnalytics.Instance.posX },
                { AnalyticsParams.posY, LevelAnalytics.Instance.posY },
                { AnalyticsParams.hpAtDeath, LevelAnalytics.Instance.hp_at_death },
                { AnalyticsParams.manaAtDeath, LevelAnalytics.Instance.mana_at_death },
                { AnalyticsParams.deathsCount, deathsCount }


            };

            AnalyticsManager.Instance.SendEvent(AnalyticsEvents.PlayerDeath, data);
        } 
    }

    public void EnemyKilled(string levelID)
    {
        var data = new Dictionary<string, object>
        {
            { AnalyticsParams.LevelId, levelID },
            { AnalyticsParams.EnemyId, "enemy" },
            { AnalyticsParams.Duration, LevelAnalytics.Instance.enemy_duration },
            { AnalyticsParams.Attempts, LevelAnalytics.Instance.enemy_attempts },
            { AnalyticsParams.DamageTaken, LevelAnalytics.Instance.enemy_damage_taken }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.EnemyKilled, data);
    }

    public void BossFightEnd(bool result)
    {
        var data = new Dictionary<string, object>
        {
            { AnalyticsParams.BossId, "boss" },
            { AnalyticsParams.Duration, LevelAnalytics.Instance.boss_duration },
            { AnalyticsParams.Result, result },
            { AnalyticsParams.Attempts, LevelAnalytics.Instance.boss_attempts },
            { AnalyticsParams.DamageTaken, LevelAnalytics.Instance.boss_damage_taken },
            { AnalyticsParams.HPLeft, LevelAnalytics.Instance.boss_hp_left }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.BossFightEnd, data);
    }

    public void WeaponUsed(string levelId)
    {
        float hitRatio = ((LevelAnalytics.Instance.attacks_in_level + LevelAnalytics.Instance.abilities_in_level) * LevelAnalytics.Instance.kill_count) / LevelAnalytics.Instance.GetTotalEnemyKillDuration(levelId);

        var data = new Dictionary<string, object>
        {
            { AnalyticsParams.LevelId, levelId },
            { AnalyticsParams.WeaponId, "Staff" },
            { AnalyticsParams.AttacksInLevel, LevelAnalytics.Instance.attacks_in_level },
            { AnalyticsParams.AbilitiesInLevel, LevelAnalytics.Instance.abilities_in_level },
            { AnalyticsParams.HitRatio, hitRatio },
            { AnalyticsParams.KillCount, LevelAnalytics.Instance.kill_count }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.WeaponUsed, data);
    }

    public void UpgradeBoughtTotal(string levelID)
    {
        for (int i = 0; i < LevelAnalytics.Instance.upgradesDatas.Count; i++)
        {
            var list = LevelAnalytics.Instance.upgradesDatas;

            var data = new Dictionary<string, object>
            {
                { AnalyticsParams.UpgradeId, list[i].upgrade_id },
                { AnalyticsParams.Category, list[i].category },
                { AnalyticsParams.TotalCost, list[i].total_cost },
                { AnalyticsParams.LevelAfterPurchase, list[i].level_after_purchase },
                { AnalyticsParams.TotalSpentTreasures, list[i].total_spent_treasures }


            };

            AnalyticsManager.Instance.SendEvent(AnalyticsEvents.UpgradeBoughtTotal, data);
        }
    }
#endif
}
