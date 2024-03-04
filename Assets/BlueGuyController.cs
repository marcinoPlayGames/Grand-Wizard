using Unity.Burst.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueGuyController : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody2D rb;
    private int direction = 1;
    public LayerMask groundLayer;
    bool isGrounded = false;

    public float NPC_MaxHealth;
    private float NPC_Health;

    public float NPC_Damage;
    private float NPC_KillCount;

    bool isHit = false;

    [SerializeField]
    AudioSource npcHit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        NPC_Health = NPC_MaxHealth;
    }

    void Update()
    {
        if (!isHit)
        {
            MoveNPC();
        }

        //CheckGround();

        float horX = GetComponent<Rigidbody2D>().velocity.x;
        float verY = GetComponent<Rigidbody2D>().velocity.y; // Get the vertical velocity

        if (!isHit)
        {
            if (!isGrounded && verY > 0) // If moving upward (jumping)
            {
                GetComponent<Animator>().SetInteger("moveStateBlueGuy", 2); // Set jump animation
            }
            else if (!isGrounded && verY < 0)
            {
                GetComponent<Animator>().SetInteger("moveStateBlueGuy", 3); // Set fall animation
            }
            else if (isGrounded && horX > 0)
            {
                GetComponent<Animator>().SetInteger("moveStateBlueGuy", 1); // Set run animation
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (isGrounded && horX < 0)
            {
                GetComponent<Animator>().SetInteger("moveStateBlueGuy", 1); // Set run animation
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<Animator>().SetInteger("moveStateBlueGuy", 0); // Set idle animation
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
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            direction *= -1;
        }
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public void DamageNPC(float damage)
    {
        NPC_Health -= damage;
        //npcHit.Play();
        StartCoroutine(HitAnimation());

        if (NPC_Health <= 0)
        {
            Destroy(gameObject, 1f);
            NPCDeathCounter.IncrementDeathCount();
            Debug.Log(NPC_KillCount);
        }
    }

    IEnumerator HitAnimation()
    {
        isHit = true;
        GetComponent<Animator>().SetInteger("moveStateBlueGuy", 4);

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

        isHit = false;
    }

}