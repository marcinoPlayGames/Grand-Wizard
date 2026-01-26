using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();

            playerMove.DamagePlayer(200, DamageType.True);

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 8f);
            }
        }
        else if (collision.gameObject.CompareTag("NPC"))
        {
            NPCController nPCController = collision.gameObject.GetComponent<NPCController>();

            nPCController.DamageNPC(5000);

            collision.gameObject.layer = LayerMask.NameToLayer("NoCollisions");

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 8f);
            }
        }
    }
}
