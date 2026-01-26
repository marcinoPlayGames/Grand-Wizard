using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPlus : MonoBehaviour
{
    private PlayerMove playerMove;

    [SerializeField]
    AudioSource healSound;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Collider2D collider2D;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();

            healSound.Play();

            if (playerMove != null)
            {
                playerMove.HealPlayer(50f + 0.05f * playerMove.Player_MaxHealth);
            }

            playerMove.increasedRegen_HP = true;

            StartCoroutine(StartPlusTimer());
        }
    }

    IEnumerator StartPlusTimer()
    {
        spriteRenderer.enabled = false;
        collider2D.enabled = false;

        yield return new WaitForSeconds(20f);

        spriteRenderer.enabled = true;
        collider2D.enabled = true;
    }
}
