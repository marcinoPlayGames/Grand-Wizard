using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelAnalytics : MonoBehaviour
{

    

    [SerializeField]
    List<string> scenes;

    public static LevelAnalytics Instance;

    public int tryNumber = 0;

    public int deaths = 0;

    public float damageTaken = 0;

    public float damageDone = 0;

    public int coinsTotal = 0;
    public int diamondsTotal = 0;
    public int treasuresTotal = 0;

    public float hpLeft = 0;
    public float duration = 0;

    public enum deathCause
    {
        trap,
        lava,
        enemy,
        boss
    }

    [System.Serializable]
    public class causeDeaths
    {
        public deathCause cause;
        public int count;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
#if UNITY_EDITOR
        Debug.Log("Za³adowano scenê: " + scene.name);
#endif

        string sceneName_local = scene.name;

        if (sceneName != sceneName_local && scenes.Contains(sceneName_local))
        {
            sceneName = sceneName_local;

            posX = 0;
            posY = 0;
            hp_at_death = 0;
            mana_at_death = 0;
            tryNumber = 0;
            deaths = 0;
            damageTaken = 0;
            damageDone = 0;
            coinsTotal = 0;
            diamondsTotal = 0;
            treasuresTotal = 0;
            hpLeft = 0;
            duration = 0;
            causeDeathsNumber.Clear();
            attacks_in_level = 0;
            abilities_in_level = 0;
            kill_count = 0;
            enemy_attempts = 0;
            enemy_damage_taken = 0;
            enemy_duration = 0;
            boss_duration = 0;
            boss_hp_left = 0;
            boss_damage_taken = 0;
            boss_attempts = 0;
        }
    }

    public string sceneName;

    public List<causeDeaths> causeDeathsNumber = new List<causeDeaths>();

    public float posX;
    public float posY;

    public float hp_at_death;
    public float mana_at_death;

    public int attacks_in_level;
    public int abilities_in_level;

    public int kill_count;

    public int enemy_attempts;

    public float enemy_damage_taken;

    public float enemy_duration;

    public float boss_duration;

    public float boss_hp_left;

    public float boss_damage_taken;

    public int boss_attempts;

    [System.Serializable]
    public class upgradesData
    {
        public string upgrade_id;

        public upgradeCategory category;
        public enum upgradeCategory
        {
            health,
            damage,
            speed,
            resistances,
            mana
        }

        public int total_cost;

        public int level_after_purchase;

        public int total_spent_treasures;
    }

    public List<upgradesData> upgradesDatas = new List<upgradesData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetTotalEnemyKillDuration(string levelId)
    {
        if (levelId == "Level5") return boss_duration;
        else return enemy_duration;
    }
}
