using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    public float fireballSpeed = 5f;
    public GameObject fireballPrefab;
    private bool isFacingRight = true;

    private GameObject lastSpawnedFireball;

    private bool canCastFireball = true;
    public float fireballCooldown = 1.5f; // Adjust the cooldown duration as needed
    public float fireballDamage = 50;

    private PlayerMove playerMove;

    [SerializeField]
    AudioSource Fireball;


    // Start is called before the first frame update
    void Start()
    {
        // Find the PlayerMove script attached to the same GameObject
        playerMove = GetComponent<PlayerMove>();

    }

    // Update is called once per frame
    void Update()
    {

        // Check if the "1" key is pressed and the player can cast a fireball
        if (Input.GetKey(KeyCode.Alpha1) && canCastFireball)
        {
            CastFireball();
        }

        /*if (lastSpawnedFireball != null)
        {
            Vector2 rayOrigin = lastSpawnedFireball.transform.position; // Use player's position as the origin
            Vector2 rayDirection = isFacingRight ? lastSpawnedFireball.transform.right : -lastSpawnedFireball.transform.right;

            // Use a LayerMask to ignore the "Fireball" layer
            LayerMask layerMask = ~LayerMask.GetMask("Fireball");
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 0.2f, layerMask);

            // Debugging information
            Debug.DrawRay(rayOrigin, rayDirection * 0.2f, Color.red); // Draw the ray in the Scene view

            if (hit.collider != null)
            {
                // Destroy the fireball if it hits terrain or an NPC
                Destroy(lastSpawnedFireball);
                Debug.Log("Fireball Collided with: " + hit.collider.gameObject.name);
            }

            if (hit.collider != null && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("NPC")))
            {
                // Destroy the fireball if it hits terrain or an NPC
                Destroy(lastSpawnedFireball);
                Debug.Log("Fireball Collided with: " + hit.collider.gameObject.name);
            }
        }*/
    }

    void CastFireball()
    {
        // Determine the direction to cast the fireball
        isFacingRight = playerMove.IsFacingRight();
        Vector2 fireballDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Adjust the instantiation position based on the player's facing direction
        float spawnOffset = isFacingRight ? 1.5f : -1.5f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.8f, 0f);

        // Create a new fireball instance using the actual fireball prefab
        lastSpawnedFireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

        Fireball.Play();

        // Set the fireball's velocity based on the direction and speed
        Rigidbody2D fireballRb = lastSpawnedFireball.GetComponent<Rigidbody2D>();
        fireballRb.velocity = fireballDirection * fireballSpeed;
        fireballRb.gravityScale = 0f;
        FireballController fireballController = lastSpawnedFireball.GetComponent<FireballController>();
        // Destroy the fireball after a certain time to prevent cluttering the scene
        if (fireballController != null)
        {
            fireballController.SetDamage(fireballDamage);
        }

        Destroy(lastSpawnedFireball, 2f);

        StartCoroutine(FireballCooldown());
    }

    IEnumerator FireballCooldown()
    {
        // Set canCastFireball to false during the cooldown
        canCastFireball = false;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(fireballCooldown);

        // Set canCastFireball to true to allow casting again
        canCastFireball = true;
    }

    public float GetFireballDamage()
    {
        return fireballDamage;
    }
}
