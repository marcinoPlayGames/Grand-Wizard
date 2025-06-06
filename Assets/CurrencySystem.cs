using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;
    private int coins = 100;

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

    public bool HasEnoughCoins(int amount) => coins >= amount;
    public void SpendCoins(int amount) => coins -= amount;
    public int GetCoins() => coins;
}