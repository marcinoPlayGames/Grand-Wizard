using System.Collections.Generic;
using UnityEngine;

public class FireballPool : MonoBehaviour
{
    public static FireballPool Instance;

    public GameObject fireballPrefab;
    public int poolSize = 8;

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
            GameObject obj = Instantiate(fireballPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetFireball()
    {
        foreach (var fireball in pool)
        {
            if (!fireball.activeInHierarchy)
            {
                fireball.SetActive(true);
                return fireball;
            }
        }

        Debug.LogWarning("FireballPool exhausted");

        GameObject obj = Instantiate(fireballPrefab);
        obj.SetActive(false);
        pool.Add(obj);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnFireball(GameObject fireball)
    {
        fireball.SetActive(false);

        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        FireballController fc = fireball.GetComponent<FireballController>();
        if (fc != null)
            fc.ResetFireball();
    }
}