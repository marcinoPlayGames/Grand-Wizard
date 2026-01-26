using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    private bool isFacingRight = true;

    public float npcSize = 1;
    public float rayHeight = 2.5f;

    public int swordNumber = 1;
    public float verticalSpacing = -2f; // Odstęp między mieczami w dół

    public float eyesPositionOffset = 0.8f;

    bool isThrowing = false;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private NPCController npcController;

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.GetComponent<Transform>();
        }
    }

    [SerializeField]
    AudioSource swordThrowSound;

    private float detectionTimer = 0f;
    private float detectionInterval = 0.1f; // sprawdz co 0.1s

    private void Update()
    {
        if (CutsceneManager.cutscenePlaying) return;
        if (npcController.IsDead) return;

        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionInterval)
        {
            CheckForPlayer();
            detectionTimer = 0f;
        }
    }

    void CheckForPlayer()
    {
        isFacingRight = npcController.IsFacingRight();
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

        float horizontalOffset = isFacingRight ? 1.5f : -1.5f;
        Vector2 origin = eyesPosition.position + new Vector3(horizontalOffset, 0.8f, 0f);

        Vector2 boxSize = new Vector2(0.1f, rayHeight); // szerokość i wysokość "promienia"

        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, direction, detectionRange, visionMask);

        DebugDrawBoxCast(origin, boxSize, 0f, direction, detectionRange, Color.green);

        if (hit.collider != null && hit.collider.CompareTag("Player") && canThrowSword && !isThrowing)
        {
            StartCoroutine(ThrowSwordCooldown());
        }
    }

    IEnumerator ThrowSword()
    {
        if (npcController.IsDead) yield break;
        
        isFacingRight = npcController.IsFacingRight();
        Vector2 swordDirection = isFacingRight ? Vector2.right : Vector2.left;
        float spawnOffsetX = isFacingRight ? 2f * npcSize : -2f * npcSize;
        Vector2 swordFacing = isFacingRight ? Vector2.right : Vector2.left;

        swordThrowSound.Play();

        for (int i = 0; i < swordNumber; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(spawnOffsetX, eyesPositionOffset + (i * verticalSpacing), 0f);
            GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);

            if (sword != null)
            {
                Rigidbody2D rb = sword.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 0f;
                    rb.velocity = swordDirection * throwForce;
                }

                SwordController swordController = sword.GetComponent<SwordController>();
                if (swordController != null)
                    swordController.SetDamage(swordDamage, DamageType.Physical);

                SpriteRenderer sr = sword.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.flipX = swordFacing == Vector2.left;

                Destroy(sword, 2f);
            }

            // Możesz dodać delikatne opóźnienie między spawnami, np.:
            // yield return new WaitForSeconds(0.05f);
        }

        yield return null;
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
        animator.SetTrigger("NPCOpenCombat");

        // Wait for the cooldown duration before throwing the sword
        yield return new WaitForSeconds(1.5f); // Wait for animation or small delay

        // Call the ThrowSword function after cooldown
        StartCoroutine(ThrowSword());

        // Wait for the sword cooldown to finish before allowing another throw
        yield return new WaitForSeconds(0.5f); // Cooldown for the next sword throw

        // After cooldown, allow throwing again
        isThrowing = false;
    }

    public bool IsThrowing()
    {
        return isThrowing;
    }

    void DebugDrawBoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
    {
#if UNITY_EDITOR

        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector2 halfSize = size * 0.5f;

        // Oblicz rogi boxa jako Vector2
        Vector2 topLeft = origin + (Vector2)(rotation * new Vector2(-halfSize.x, halfSize.y));
        Vector2 topRight = origin + (Vector2)(rotation * new Vector2(halfSize.x, halfSize.y));
        Vector2 bottomLeft = origin + (Vector2)(rotation * new Vector2(-halfSize.x, -halfSize.y));
        Vector2 bottomRight = origin + (Vector2)(rotation * new Vector2(halfSize.x, -halfSize.y));

        Vector2 move = direction.normalized * distance;

        // Przesunięte rogi
        Vector2 tlMoved = topLeft + move;
        Vector2 trMoved = topRight + move;
        Vector2 blMoved = bottomLeft + move;
        Vector2 brMoved = bottomRight + move;

        // Rysuj box przed ruchem
        Debug.DrawLine((Vector3)topLeft, (Vector3)topRight, color);
        Debug.DrawLine((Vector3)topRight, (Vector3)bottomRight, color);
        Debug.DrawLine((Vector3)bottomRight, (Vector3)bottomLeft, color);
        Debug.DrawLine((Vector3)bottomLeft, (Vector3)topLeft, color);

        // Rysuj box po ruchu
        Debug.DrawLine((Vector3)tlMoved, (Vector3)trMoved, color);
        Debug.DrawLine((Vector3)trMoved, (Vector3)brMoved, color);
        Debug.DrawLine((Vector3)brMoved, (Vector3)blMoved, color);
        Debug.DrawLine((Vector3)blMoved, (Vector3)tlMoved, color);

        // Rysuj połączenia między boxami (krawędzie "tunelu")
        Debug.DrawLine((Vector3)topLeft, (Vector3)tlMoved, color);
        Debug.DrawLine((Vector3)topRight, (Vector3)trMoved, color);
        Debug.DrawLine((Vector3)bottomLeft, (Vector3)blMoved, color);
        Debug.DrawLine((Vector3)bottomRight, (Vector3)brMoved, color);

#endif
    }
}
