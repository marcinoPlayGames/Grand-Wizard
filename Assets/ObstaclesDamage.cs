using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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

            playerMove.DamagePlayer(50);

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null && collision.contacts.Length > 0)
            {
                // Pobierz normalną pierwszego kontaktu
                Vector2 normal = collision.contacts[0].normal;

                // Wektor przeciwny do normalnej, który może posłużyć jako kierunek wyrzutu
                Vector2 knockbackDir = -normal.normalized;

                // Skaluje siłę wyrzutu – można dostosować
                float knockbackForce = 6f;

                rb.velocity = knockbackDir * knockbackForce;
            }
        }
    }
}
