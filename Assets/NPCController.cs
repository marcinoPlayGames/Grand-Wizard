using Unity.Burst.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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

    public event Action OnNPCHealthChange;

    [SerializeField]
    private string loreSceneName;

    [SerializeField]
    private string enemyId;

    private bool durationStart;

    private float duration;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        NPC_Health = NPC_MaxHealth;

        npcAI = GetComponent<NPCAI>();
    }

    void Awake()
    {
        if (npcDeathCounter == null)
        {
            GameObject npcObj = GameObject.Find("NPCDeathCounter");
            if (npcObj != null)
                npcDeathCounter = npcObj.GetComponent<NPCDeathCounter>();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == loreSceneName) return;

        isThrowing = npcAI.IsThrowing();

        if (!isHit && !isThrowing)
        {
            MoveNPC();
        }

        CheckForEdge();

        //CheckGround();

        float horX = GetComponent<Rigidbody2D>().velocity.x;
        float verY = GetComponent<Rigidbody2D>().velocity.y; // Get the vertical velocity

        if (durationStart) duration += Time.time;

        if (!isHit && !isThrowing)
        {
            //Debug.Log("moveState = " + GetComponent<Animator>().GetInteger("moveStateNPC"));
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
        if (CutsceneManager.cutscenePlaying) return;
        if (isDead) return;

        durationStart = true;

        NPC_Health -= damage;
        npcHit.Play();
        StartCoroutine(HitAnimation());

        if (enemyId == "boss")
        {
            LevelAnalytics.Instance.boss_damage_taken += damage;
            GameManager.Instance.boss_hp_left = NPC_Health;
            GameManager.Instance.boss_duration = duration;
        }
        else
        {
            LevelAnalytics.Instance.enemy_damage_taken += damage;
            LevelAnalytics.Instance.enemy_attempts += 1;
        }

        if (NPC_Health <= 0)
        {
            NPC_Health = 0;
            
            isDead = true;
            Destroy(gameObject, 1f);

            LevelAnalytics.Instance.kill_count += 1;
            LevelAnalytics.Instance.enemy_duration += duration;
            NPCManager.Instance.EnemyDied();
            Debug.Log(NPC_KillCount);
        }

        Debug.Log($"[NPCController] NPC HP: {NPC_Health}");

        OnNPCHealthChange?.Invoke();
    }

    public float GetNPCHealth()
    {
        return NPC_Health;
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
