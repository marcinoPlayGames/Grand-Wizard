using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDamage : MonoBehaviour
{
    bool canDamage = true;
    // Start is called before the first frame update

    [SerializeField]
    private Rigidbody2D rb;

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collided with something!");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with player!");
            PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();

            // Sprawdź, gdzie znajdują się kolce
            Vector3 spikePosition = collision.gameObject.transform.position;

            Debug.Log("spikePosition = " +  spikePosition);

            // Użyj Raycast, aby sprawdzić, czy w tym miejscu znajduje się teren
            RaycastHit2D hit = Physics2D.Raycast(spikePosition, Vector2.down, 2f, LayerMask.GetMask("Teren"));

            Debug.DrawRay(spikePosition, Vector2.down * 0.1f, Color.yellow);

            RaycastHit2D hit2 = Physics2D.Raycast(spikePosition, Vector2.down, 2f, LayerMask.GetMask("Obstacles"));

            Debug.DrawRay(spikePosition, Vector2.down * 0.1f, Color.yellow);
            if (hit.collider != null && hit2.collider != null && hit.collider.CompareTag("Wall") && hit2.collider.CompareTag("Obstacles"))
            {
                Debug.Log("Collided both!");
                canDamage = false;
            }
            else canDamage = true;
            if (canDamage)
            {
                Debug.Log("Collided with player!");

                playerMove.DamagePlayer(50, DamageType.Physical);

                if (rb != null && collision.contacts.Length > 0)
                {
                    // Pobierz normalną pierwszego kontaktu
                    Vector2 normal = collision.contacts[0].normal;

                    // Wektor przeciwny do normalnej, który może posłużyć jako kierunek wyrzutu
                    Vector2 knockbackDir = -normal.normalized;

                    // Skaluje siłę wyrzutu – można dostosować
                    float knockbackForce = 6f;

                    if (normal.y < 0) // Normalna wskazuje na dolną stronę obiektu
                    {
                        rb.velocity = knockbackDir * knockbackForce;
                    }
                }
            }
        }
    }
}
