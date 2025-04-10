using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LavaDamage script loaded on: " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with something!");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with player!");
            PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();

            playerMove.DamagePlayer(200);

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 8f);
            }
        }
    }
}
