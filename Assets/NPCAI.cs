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

        // Offset the ray spawn position based on NPC facing direction
        float spawnOffset = isFacingRight ? 1.5f : -1.5f;
        Vector3 spawnPosition = eyesPosition.position + new Vector3(spawnOffset, 0.8f, 0f);

        // Set ray direction based on NPC facing direction
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Cast a ray in front of the NPC
        RaycastHit2D hit = Physics2D.Raycast(spawnPosition, rayDirection, detectionRange, visionMask);

        // Debug the ray in Scene view (optional)
        Debug.DrawRay(spawnPosition, rayDirection * detectionRange, Color.red);

        // Check if the ray hits the player
        if (hit.collider != null && hit.collider.CompareTag("Player") && canThrowSword)
        {
            // Throw the sword towards the player
            ThrowSword(rayDirection);
        }
    }

    void ThrowSword(Vector2 direction)
    {
        // Instantiate the sword at enemy's position
        float spawnOffset = isFacingRight ? 1.5f : -1.5f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.8f, 0f);

        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Sword throwed");

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

        Destroy(sword, 2f);

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
}
