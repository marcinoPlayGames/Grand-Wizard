using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinAmount = 1;

    [SerializeField]
    AudioSource coinCollect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collected coin!");
            
            coinCollect.Play();
            GameManager.Instance.AddCoin(coinAmount);

            GetComponent<SpriteRenderer>().enabled = false; // Hide the coin
            GetComponent<Collider2D>().enabled = false; // Disable collision

            Destroy(gameObject, coinCollect.clip.length);

            LevelAnalytics.Instance.treasuresTotal += coinAmount;
            GameManager.Instance.treasuresCollected += coinAmount;

            if (coinAmount > 1) LevelAnalytics.Instance.diamondsTotal += coinAmount;
            else LevelAnalytics.Instance.coinsTotal += coinAmount;

            if (coinAmount > 1) GameManager.Instance.diamondsCollected += coinAmount;
            else GameManager.Instance.coinsCollected += coinAmount;
        }
    }
}
