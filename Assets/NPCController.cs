using Unity.Burst.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody2D rb;
    private int direction = 1;
    public LayerMask groundLayer;
    public NPCDeathCounter npcDeathCounter;
    bool isGrounded = false;

    public float NPC_MaxHealth;
    private float NPC_Health;

    public float NPC_Damage;
    private float NPC_KillCount;

    private bool isFacingRight = true;

    bool isHit = false;
    bool isThrowing = false;

    private NPCAI npcAI;

    private bool isDead = false;

    [SerializeField]
    AudioSource npcHit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        NPC_Health = NPC_MaxHealth;

        npcAI = GetComponent<NPCAI>();
    }

    void Update()
    {
        isThrowing = npcAI.IsThrowing();

        if (!isHit && !isThrowing)
        {
            MoveNPC();
        }

        CheckForEdge();

        //CheckGround();

        float horX = GetComponent<Rigidbody2D>().velocity.x;
        float verY = GetComponent<Rigidbody2D>().velocity.y; // Get the vertical velocity

        

        if (!isHit && !isThrowing)
        {
            Debug.Log("moveState = " + GetComponent<Animator>().GetInteger("moveStateNPC"));
            if (!isGrounded && verY > 0)
            {
                GetComponent<Animator>().SetInteger("moveStateNPC", 3); // Set fall animation
            }
            else if (!isGrounded && verY < 0)
            {
                GetComponent<Animator>().SetInteger("moveStateNPC", 3); // Set fall animation
            }
            else if (isGrounded && horX > 0)
            {
                GetComponent<Animator>().SetInteger("moveStateNPC", 1); // Set run animation
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (isGrounded && horX < 0)
            {
                GetComponent<Animator>().SetInteger("moveStateNPC", 1); // Set run animation
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<Animator>().SetInteger("moveStateNPC", 0); // Set idle animation
            }
        }
        
    }

    void MoveNPC()
    {
        Vector3 movement = new Vector3(speed * direction, rb.velocity.y, 0);
        rb.velocity = movement;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = true;
        }

        Vector3 velocity2 = rb.velocity;
        Vector3 velocity3 = new Vector3(0, 0, 0);
        if (velocity2 == velocity3)
        {
            direction *= -1;
            isFacingRight = !isFacingRight;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            direction *= -1;
            isFacingRight = !isFacingRight;
        }
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public void DamageNPC(float damage)
    {
        if (isDead) return;
        
        NPC_Health -= damage;
        npcHit.Play();
        StartCoroutine(HitAnimation());

        if (NPC_Health <= 0)
        {
            isDead = true;
            Destroy(gameObject, 1f);
            npcDeathCounter.IncrementDeathCount();
            npcDeathCounter.CheckEndingCondition();
            Debug.Log(NPC_KillCount);
        }
    }

    IEnumerator HitAnimation()
    {
        isHit = true;
        GetComponent<Animator>().SetInteger("moveStateNPC", 4);

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

        isHit = false;
    }

    public bool IsFacingRight()
    {
        return GetComponent<SpriteRenderer>().flipX == false;
    }

    void CheckForEdge()
    {
        float edgeCheckDistance = 1.0f; // Distance to check ahead of NPC
        Vector2 rayOrigin = new Vector2(transform.position.x + (isFacingRight ? edgeCheckDistance : -edgeCheckDistance), transform.position.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 3.0f, groundLayer);
        Debug.DrawRay(rayOrigin, Vector2.down * 3.0f, Color.blue);

        if (hit.collider == null)
        {
            // No ground detected, turn around
            isFacingRight = !isFacingRight;
            Flip();
        }
    }

    void Flip()
    {
        //Vector3 localScale = transform.localScale;
        //localScale.x *= -1;
        //transform.localScale = localScale;

        direction *= -1;
    }


}
