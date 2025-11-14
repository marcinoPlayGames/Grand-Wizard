using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int coins = 0;
    public TMP_Text coinText;

    public int unlockedLevel = 8; // zaczynamy od Level 1

    public int GetUnlockedLevel() => unlockedLevel;
    public void SetUnlockedLevel(int value) => unlockedLevel = value;


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

    private void Start()
    {
        UpdateUI();
    }

    public void AddCoin(int coin)
    {
        coins = coins + coin;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText = GameObject.Find("CoinText").GetComponent<TMP_Text>();
        }
        
        if (coinText != null) coinText.text = "" + coins;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUI();
    }

    public bool HasEnoughCoins(int amount) => coins >= amount;
    public void SpendCoins(int amount) => coins -= amount;
    public int GetCoins() => coins;

    [ContextMenu("Add 800 Coins")]
    public void DebugAddCoins()
    {
        AddCoin(800);
        Debug.Log("Coins: " + coins);
    }

    public void ResetCoins()
    {
        coins = 0;
    }

    public void ResetUnlockedLevel()
    {
        unlockedLevel = 8;
    }
}
