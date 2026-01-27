using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    [SerializeField] private string playerType;
    public string LevelID = string.Empty;

#if USE_ANALYTICS
    private readonly Dictionary<string, object> reusableData = new Dictionary<string, object>(32);
    private string cachedResolution;
#endif

    // FPS tracking
    float fpsSum = 0f;
    int fpsSamples = 0;
    float fpsMin = 0f;

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
    }
#endif

    void Start()
    {
#if USE_ANALYTICS
        cachedResolution = $"{Screen.currentResolution.width}x{Screen.currentResolution.height}";
#endif
    }

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
        try
        {
            var ev = new CustomEvent(eventName);
            foreach (var kv in parameters)
                ev[kv.Key] = kv.Value;

            AnalyticsService.Instance.RecordEvent(ev);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[Analytics] Failed to send event '{eventName}': {e.Message}");
#endif
        }
    }
#endif

    private void OnApplicationQuit()
    {
#if USE_ANALYTICS
        EndSession();
        EndsLevel(false, LevelID);
        PlayerDeath(LevelID);
        EnemyKilled(LevelID);
        BossFightEnd(false);
        WeaponUsed(LevelID);
        UpgradeBoughtTotal(LevelID);
#endif
    }

#if USE_ANALYTICS
    void EndSession()
    {
        float avgFps = fpsSamples > 0 ? fpsSum / fpsSamples : 0f;
        avgFps = (float)Math.Round(avgFps, 2);

        reusableData.Clear();
        reusableData[AnalyticsParams.Duration] = Time.time;
        reusableData[AnalyticsParams.LevelsCompleted] = GameManager.Instance.levelsCompleted;
        reusableData[AnalyticsParams.CoinsTotal] = GameManager.Instance.coinsCollected;
        reusableData[AnalyticsParams.DiamondsTotal] = GameManager.Instance.diamondsCollected;
        reusableData[AnalyticsParams.TreasuresTotal] = GameManager.Instance.treasuresCollected;
        reusableData[AnalyticsParams.FPSAvg] = avgFps;
        reusableData[AnalyticsParams.PlayerType] = playerType;

        SendEvent(AnalyticsEvents.SessionEnd, reusableData);
    }

    public void PerformanceAnalytics(string levelID)
    {
        float fpsAvg = fpsSamples > 0 ? fpsSum / fpsSamples : 0f;

        reusableData.Clear();
        reusableData[AnalyticsParams.LevelId] = levelID;
        reusableData[AnalyticsParams.FPSAvg] = fpsAvg;
        reusableData[AnalyticsParams.FPSMin] = fpsMin;
        reusableData[AnalyticsParams.DeviceModel] = SystemInfo.deviceModel;
        reusableData[AnalyticsParams.Resolution] = cachedResolution;

        SendEvent(AnalyticsEvents.Performance, reusableData);
    }

    public void EndsLevel(bool result, string levelID)
    {
        reusableData.Clear();
        reusableData[AnalyticsParams.LevelId] = levelID;
        reusableData[AnalyticsParams.TryNumber] = LevelAnalytics.Instance.tryNumber;
        reusableData[AnalyticsParams.Duration] = LevelAnalytics.Instance.duration;
        reusableData[AnalyticsParams.Result] = result;
        reusableData[AnalyticsParams.Deaths] = LevelAnalytics.Instance.deaths;
        reusableData[AnalyticsParams.DamageTaken] = LevelAnalytics.Instance.damageTaken;
        reusableData[AnalyticsParams.DamageDone] = LevelAnalytics.Instance.damageDone;
        reusableData[AnalyticsParams.CoinsTotal] = LevelAnalytics.Instance.coinsTotal;
        reusableData[AnalyticsParams.DiamondsTotal] = LevelAnalytics.Instance.diamondsTotal;
        reusableData[AnalyticsParams.TreasuresTotal] = LevelAnalytics.Instance.treasuresTotal;
        reusableData[AnalyticsParams.HPLeft] = LevelAnalytics.Instance.hpLeft;
        reusableData[AnalyticsParams.UsedUpgrades] = WeaponUpgradeSystem.Instance.GetLevel("Staff");

        SendEvent(AnalyticsEvents.LevelEnd, reusableData);
    }

    public void PlayerDeath(string levelID)
    {
        var list = LevelAnalytics.Instance.causeDeathsNumber;

        for (int i = 0; i < list.Count; i++)
        {
            reusableData.Clear();
            reusableData[AnalyticsParams.LevelId] = levelID;
            reusableData[AnalyticsParams.Cause] = list[i].cause;
            reusableData[AnalyticsParams.deathsCount] = list[i].count;
            reusableData[AnalyticsParams.posX] = LevelAnalytics.Instance.posX;
            reusableData[AnalyticsParams.posY] = LevelAnalytics.Instance.posY;
            reusableData[AnalyticsParams.hpAtDeath] = LevelAnalytics.Instance.hp_at_death;
            reusableData[AnalyticsParams.manaAtDeath] = LevelAnalytics.Instance.mana_at_death;

            SendEvent(AnalyticsEvents.PlayerDeath, reusableData);
        }
    }

    public void EnemyKilled(string levelID)
    {
        reusableData.Clear();
        reusableData[AnalyticsParams.LevelId] = levelID;
        reusableData[AnalyticsParams.EnemyId] = "enemy";
        reusableData[AnalyticsParams.Duration] = LevelAnalytics.Instance.enemy_duration;
        reusableData[AnalyticsParams.Attempts] = LevelAnalytics.Instance.enemy_attempts;
        reusableData[AnalyticsParams.DamageTaken] = LevelAnalytics.Instance.enemy_damage_taken;

        SendEvent(AnalyticsEvents.EnemyKilled, reusableData);
    }

    public void BossFightEnd(bool result)
    {
        reusableData.Clear();
        reusableData[AnalyticsParams.BossId] = "boss";
        reusableData[AnalyticsParams.Duration] = LevelAnalytics.Instance.boss_duration;
        reusableData[AnalyticsParams.Result] = result;
        reusableData[AnalyticsParams.Attempts] = LevelAnalytics.Instance.boss_attempts;
        reusableData[AnalyticsParams.DamageTaken] = LevelAnalytics.Instance.boss_damage_taken;
        reusableData[AnalyticsParams.HPLeft] = LevelAnalytics.Instance.boss_hp_left;

        SendEvent(AnalyticsEvents.BossFightEnd, reusableData);
    }

    public void WeaponUsed(string levelId)
    {
        float hitRatio =
            ((LevelAnalytics.Instance.attacks_in_level + LevelAnalytics.Instance.abilities_in_level)
            * LevelAnalytics.Instance.kill_count)
            / LevelAnalytics.Instance.GetTotalEnemyKillDuration(levelId);

        reusableData.Clear();
        reusableData[AnalyticsParams.LevelId] = levelId;
        reusableData[AnalyticsParams.WeaponId] = "Staff";
        reusableData[AnalyticsParams.AttacksInLevel] = LevelAnalytics.Instance.attacks_in_level;
        reusableData[AnalyticsParams.AbilitiesInLevel] = LevelAnalytics.Instance.abilities_in_level;
        reusableData[AnalyticsParams.HitRatio] = hitRatio;
        reusableData[AnalyticsParams.KillCount] = LevelAnalytics.Instance.kill_count;

        SendEvent(AnalyticsEvents.WeaponUsed, reusableData);
    }

    public void UpgradeBoughtTotal(string levelID)
    {
        var list = LevelAnalytics.Instance.upgradesDatas;

        for (int i = 0; i < list.Count; i++)
        {
            reusableData.Clear();
            reusableData[AnalyticsParams.LevelId] = levelID;
            reusableData[AnalyticsParams.UpgradeId] = list[i].upgrade_id;
            reusableData[AnalyticsParams.Category] = list[i].category;
            reusableData[AnalyticsParams.TotalCost] = list[i].total_cost;
            reusableData[AnalyticsParams.LevelAfterPurchase] = list[i].level_after_purchase;
            reusableData[AnalyticsParams.TotalSpentTreasures] = list[i].total_spent_treasures;

            SendEvent(AnalyticsEvents.UpgradeBoughtTotal, reusableData);
        }
    }
#endif
}