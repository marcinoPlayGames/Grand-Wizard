using Unity.Burst.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class NPCController : MonoBehaviour
{
    public float speed = 1f;
    
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

    private bool isDead = false;

    public bool IsDead {get {return isDead;}}

    [SerializeField]
    AudioSource npcHit;

    public event Action OnNPCHealthChange;

    [SerializeField]
    private string loreSceneName;

    [SerializeField]
    private string enemyId;

    private bool durationStart;

    private float duration; 

    [SerializeField] private LayerMask collisionLayers;

    [SerializeField] private LayerMask wallLayers;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private NPCAI npcAI;

    [SerializeField] private Animator animator;

    [SerializeField] private SpriteRenderer sr;

    float lastSpeed;

    float lastVelocity;

    bool wasGrounded;

    private float detectionTimer = 0f;
    private float detectionInterval = 0.1f;

    private EnemyHealthBarUI healthBar;

    void OnEnable()
    {
        healthBar = EnemyHealthBarPool.Instance.Get();
        healthBar.Bind(this);
    }

    void Start()
    {
        NPC_Health = NPC_MaxHealth;
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
        if (CutsceneManager.cutscenePlaying) return;

        isThrowing = npcAI.IsThrowing();

        if (!isHit && !isThrowing)
        {
            MoveNPC();
        }

        float horX = rb.velocity.x;
        float verY = rb.velocity.y; // Get the vertical velocity

        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionInterval)
        {
            CheckForEdge();
            detectionTimer = 0f;
        }

        horX = Mathf.Abs(horX);

        if (durationStart) duration += Time.deltaTime;

        if (!isHit && !isThrowing)
        {
            if (lastVelocity != verY)
            {
                lastVelocity = verY;
                animator.SetFloat("VelocityVertical", lastVelocity);
            }
            if (lastSpeed != horX)
            {
                lastSpeed = Math.Abs(horX);
                animator.SetFloat("Speed", lastSpeed);
            }
            if (wasGrounded != isGrounded)
            {
                wasGrounded = isGrounded;
                animator.SetBool("IsGrounded", wasGrounded);
            }
        }
    }

    void MoveNPC()
    {
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (SceneManager.GetActiveScene().name == loreSceneName) return;
        if (CutsceneManager.cutscenePlaying) return;
        if (isHit || isThrowing) return;

        if (((1 << collision.gameObject.layer) & wallLayers) != 0)
        {
            
            isGrounded = true;

            Vector3 velocity2 = rb.velocity;
            Vector3 velocity3 = new Vector3(0, 0, 0);
            if (velocity2 == velocity3)
            {
                Flip();
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (SceneManager.GetActiveScene().name == loreSceneName) return;
        if (CutsceneManager.cutscenePlaying) return;

        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            Flip();
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

            StartCoroutine(DeactiveNPC());

            LevelAnalytics.Instance.kill_count += 1;
            LevelAnalytics.Instance.enemy_duration += duration;
            NPCManager.Instance.EnemyDied();
        }

        OnNPCHealthChange?.Invoke();
    }

    IEnumerator DeactiveNPC()
    {
        if (healthBar != null)
        {
            EnemyHealthBarPool.Instance.Return(healthBar);
            healthBar = null;
        }

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }

    public float GetNPCHealth()
    {
        return NPC_Health;
    }

    IEnumerator HitAnimation()
    {
        isHit = true;
        animator.SetTrigger("NPCHit");

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

        isHit = false;
    }

    public bool IsFacingRight()
    {
        return sr.flipX == false;
    }

    void CheckForEdge()
    {
        if (SceneManager.GetActiveScene().name == loreSceneName) return;
        if (CutsceneManager.cutscenePlaying) return;

        float size = transform.localScale.x;
        
        float edgeCheckDistance = 1.0f; // Distance to check ahead of NPC
        Vector2 rayOrigin = new Vector2(transform.position.x + (isFacingRight ? edgeCheckDistance : -edgeCheckDistance), transform.position.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 3.0f * size, groundLayer);

#if UNITY_EDITOR
        Debug.DrawRay(rayOrigin, Vector2.down * 3.0f * size, Color.blue);
#endif

        if (hit.collider == null)
        {
            
            // No ground detected, turn around
            Flip();
        }
    }

    void Flip()
    {
        if (SceneManager.GetActiveScene().name == loreSceneName) return;
        if (CutsceneManager.cutscenePlaying) return;

        direction *= -1;
        isFacingRight = !isFacingRight;

        sr.flipX = !isFacingRight;
    }


}
