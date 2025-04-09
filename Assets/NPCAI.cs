using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAI : MonoBehaviour
{
    public Transform player;         // Assign player in the Inspector
    public Transform eyesPosition;   // The point from which the enemy "sees" (set this to the eye level)
    public GameObject swordPrefab;   // Prefab of the sword to throw
    public float detectionRange = 10f; // Max distance the enemy can "see"
    public float throwForce = 10f;   // Force applied to the sword when thrown
    public LayerMask visionMask;     // Layer mask to filter what the enemy can "see"
    public float swordDamage = 100;
    public float swordCooldown = 1f;
    private bool canThrowSword = true;
    private NPCController npcController;
    private bool isFacingRight = true;

    bool isThrowing = false;

    [SerializeField]
    AudioSource swordThrowSound;

    void Start()
    {
        // Find the PlayerMove script attached to the same GameObject
        npcController = GetComponent<NPCController>();

    }

    private void Update()
    {
        CheckForPlayer();
    }

    void CheckForPlayer()
    {
        // Get NPC's facing direction
        isFacingRight = npcController.IsFacingRight();

        // Set ray direction based on NPC facing direction
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Offset the ray spawn position based on NPC facing direction
        float spawnOffset = isFacingRight ? 1.5f : -1.5f;
        Vector3 spawnPosition = eyesPosition.position + new Vector3(spawnOffset, 0.8f, 0f);

        // Cast a ray in front of the NPC
        RaycastHit2D hit = Physics2D.Raycast(spawnPosition, rayDirection, detectionRange, visionMask);

        // Debug the ray in Scene view (optional)
        Debug.DrawRay(spawnPosition, rayDirection * detectionRange, Color.red);

        // Check if the ray hits the player and cooldown is finished
        if (hit.collider != null && hit.collider.CompareTag("Player") && canThrowSword && !isThrowing)
        {
            // Start the ThrowSwordCooldown if not already throwing
            StartCoroutine(ThrowSwordCooldown());
        }
    }

    /*void ThrowSword(Vector2 direction)
    {
        // Instantiate the sword at enemy's position
        float spawnOffset = isFacingRight ? 1.8f : -1.8f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.8f, 0f);
        Vector2 swordFacing = isFacingRight ? Vector2.right : Vector2.left;

        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Sword throwed");

        

        swordThrowSound.Play();

        // Add force to make it move toward the player
        Rigidbody2D rb = sword.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * throwForce;
        }

        SwordController6 swordController6 = sword.GetComponent<SwordController6>();

        // Destroy the fireball after a certain time to prevent cluttering the scene
        if (swordController6 != null)
        {
            swordController6.SetDamage(swordDamage);
        }

        if (swordFacing == new Vector2(-1.00f, 0.00f))
            sword.GetComponent<SpriteRenderer>().flipX = true;
        else
            sword.GetComponent<SpriteRenderer>().flipX = false;

        Destroy(sword, 2f);

        StartCoroutine(SwordCooldown());
    }*/

    /*void ThrowSword(Vector2 direction)
    {
        // Zmień pozycję w zależności od kierunku
        float spawnOffset = isFacingRight ? 3f : -3f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.8f, 0f);

        // Instancjonowanie miecza
        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Sword thrown");

        // Dźwięk rzutu miecza
        swordThrowSound.Play();

        // Dodaj siłę do ruchu miecza
        Rigidbody2D rb = sword.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * throwForce;
        }

        // Przypisanie obrażeń do miecza
        SwordController6 swordController6 = sword.GetComponent<SwordController6>();
        if (swordController6 != null)
        {
            swordController6.SetDamage(swordDamage);
        }

        // Zmiana kierunku sprite'a, jeśli jest to konieczne
        if (direction == new Vector2(-1.00f, 0.00f))
            sword.GetComponent<SpriteRenderer>().flipX = true;
        else
            sword.GetComponent<SpriteRenderer>().flipX = false;

        // Zniszczenie miecza po 2 sekundach
        Destroy(sword, 2f);
        StartCoroutine(SwordCooldown());
    }*/

    IEnumerator ThrowSword()
    {
        Debug.Log("Sword thrown!");
        isFacingRight = npcController.IsFacingRight();
        Vector2 swordDirection = isFacingRight ? Vector2.right : Vector2.left;
        float spawnOffset = isFacingRight ? 2f : -2f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.8f, 0f);
        Vector2 swordFacing = isFacingRight ? Vector2.right : Vector2.left;

        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        swordThrowSound.Play();

        yield return null;

        if (sword != null)
        {
            Rigidbody2D rb = sword.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.velocity = swordDirection * throwForce;
            }

            SwordController6 swordController = sword.GetComponent<SwordController6>();
            if (swordController != null)
                swordController.SetDamage(swordDamage);

            if (swordFacing == new Vector2(-1.00f, 0.00f))
                sword.GetComponent<SpriteRenderer>().flipX = true;
            else
                sword.GetComponent<SpriteRenderer>().flipX = false;

            Destroy(sword, 2f);
        }

        StartCoroutine(SwordCooldown());
    }

    IEnumerator SwordCooldown()
    {
        // Set canCastFireball to false during the cooldown
        canThrowSword = false;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(swordCooldown);

        // Set canCastFireball to true to allow casting again
        canThrowSword = true;
    }

    IEnumerator ThrowSwordCooldown()
    {
        // Set isThrowing flag to prevent multiple throws
        isThrowing = true;

        // Play sword throwing animation
        GetComponent<Animator>().SetTrigger("ThrowSword");

        // Wait for the cooldown duration before throwing the sword
        yield return new WaitForSeconds(0.1f); // Wait for animation or small delay

        // Call the ThrowSword function after cooldown
        StartCoroutine(ThrowSword());

        // Wait for the sword cooldown to finish before allowing another throw
        yield return new WaitForSeconds(0.2f); // Cooldown for the next sword throw

        // After cooldown, allow throwing again
        isThrowing = false;
    }

    public bool IsThrowing()
    {
        return isThrowing;
    }
}
