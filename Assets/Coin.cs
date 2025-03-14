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
            coinCollect.Play();
            GameManager.Instance.AddCoin(coinAmount);

            GetComponent<SpriteRenderer>().enabled = false; // Hide the coin
            GetComponent<Collider2D>().enabled = false; // Disable collision

            Destroy(gameObject, coinCollect.clip.length);
        }
    }
}
