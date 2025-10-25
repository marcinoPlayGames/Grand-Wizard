using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPlus : MonoBehaviour
{
    private PlayerMove playerMove;

    [SerializeField]
    AudioSource healSound;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();

            healSound.Play();

            if (playerMove != null)
            {
                Debug.Log("Collided!");

                playerMove.HealPlayer(50f + 0.05f * playerMove.Player_MaxHealth);
            }

            Debug.Log(gameObject.name);

            playerMove.increasedRegen_HP = true;

            StartCoroutine(StartPlusTimer());
        }
    }

    IEnumerator StartPlusTimer()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(20f);

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
