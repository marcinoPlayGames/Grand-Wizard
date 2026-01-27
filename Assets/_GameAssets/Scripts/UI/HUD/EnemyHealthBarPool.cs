using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarPool : MonoBehaviour
{
    public static EnemyHealthBarPool Instance;

    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] int poolSize = 40;

    private Queue<EnemyHealthBarUI> pool = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(healthBarPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<EnemyHealthBarUI>());
        }
    }

    public EnemyHealthBarUI Get()
    {
        if (pool.Count == 0)
            return Instantiate(healthBarPrefab, transform)
                   .GetComponent<EnemyHealthBarUI>();

        var bar = pool.Dequeue();
        bar.gameObject.SetActive(true);
        return bar;
    }

    public void Return(EnemyHealthBarUI bar)
    {
        bar.ResetUI();
        bar.gameObject.SetActive(false);
        pool.Enqueue(bar);
    }
}