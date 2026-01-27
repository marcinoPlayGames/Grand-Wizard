using System.Collections.Generic;
using UnityEngine;

public class FireballPool : MonoBehaviour
{
    public static FireballPool Instance;

    [SerializeField]
    private List<FireballPoolConfig> fireballConfigs;

    private Dictionary<FireballType, List<GameObject>> pools;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        pools = new Dictionary<FireballType, List<GameObject>>();

        foreach (var config in fireballConfigs)
        {
            List<GameObject> pool = new List<GameObject>();

            for (int i = 0; i < config.poolSize; i++)
            {
                GameObject obj = Instantiate(config.prefab);
                obj.SetActive(false);
                pool.Add(obj);
            }

            pools.Add(config.type, pool);
        }
    }

    public GameObject GetFireball(FireballType type)
    {
        if (!pools.ContainsKey(type))
        {
            Debug.LogError($"No pool for FireballType: {type}");
            return null;
        }

        foreach (var fireball in pools[type])
        {
            if (!fireball.activeInHierarchy)
            {
                fireball.SetActive(true);
                return fireball;
            }
        }

        Debug.LogWarning($"FireballPool exhausted for type {type}");

        var prefab = fireballConfigs.Find(c => c.type == type).prefab;
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pools[type].Add(obj);
        obj.SetActive(true);

        return obj;
    }

    public void ReturnFireball(FireballType type, GameObject fireball)
    {
        fireball.SetActive(false);

        if (fireball.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (fireball.TryGetComponent(out FireballController fc))
            fc.ResetFireball();
    }
}