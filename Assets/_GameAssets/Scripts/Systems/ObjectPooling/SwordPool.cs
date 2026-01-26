using System.Collections.Generic;
using UnityEngine;

public class SwordPool : MonoBehaviour
{
    public static SwordPool Instance; // ⬅️ TO BYŁO BRAKUJĄCE

    public GameObject swordPrefab;
    public int poolSize = 20;

    private List<GameObject> pool;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(swordPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetSword()
    {
        foreach (var sword in pool)
        {
            if (!sword.activeInHierarchy)
            {
                sword.SetActive(true);
                return sword;
            }
        }

        Debug.LogWarning("SwordPool exhausted");

        GameObject obj = Instantiate(swordPrefab);
        obj.SetActive(false);
        pool.Add(obj);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnSword(GameObject sword)
    {
        sword.SetActive(false);

        Rigidbody2D rb = sword.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        SwordController sc = sword.GetComponent<SwordController>();
        if (sc != null)
            sc.ResetSword();
    }
}