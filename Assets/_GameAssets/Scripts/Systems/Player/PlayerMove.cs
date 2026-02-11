using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private PlayerAttacks playerAttacks;
    [SerializeField]
    private StatHealth statHealth;
    [SerializeField]
    private StatDefenses statDefenses;
    [SerializeField]
    private Collider2D col;
    [SerializeField]
    private StatMana statMana;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform transformPlayer;

    private DamageType damageType;

    public PhysicsMaterial2D groundedMaterial;
    public PhysicsMaterial2D airMaterial;

    private float Mana;
    public float MaxMana;

    public bool increasedRegen_HP;

    [SerializeField] float maxSpeed = 6f;            // maksymalna prędkość pozioma
    [SerializeField] float groundAcceleration = 50f; // przyspieszenie na ziemi
    [SerializeField] float airAcceleration = 20f;    // przyspieszenie w powietrzu
    [SerializeField] float groundDeceleration = 30f; // hamowanie na ziemi
    [SerializeField] float airDeceleration = 5f;     // hamowanie w powietrzu  

    [SerializeField]
    AudioSource playerAudioSource;

    [SerializeField]
    AudioClip playerJump;

    [SerializeField]
    AudioClip playerHit;

    [SerializeField]
    AudioClip playerFall;

    int count = 0;
    // Update is called once per frame
    bool jumpState = false;
    bool isGrounded = false;

    public float Player_MaxHealth;
    private float Player_Health;
    bool isHit = false;

    bool isCasting = false;
    bool isCastAnimation = false;

    bool canObstacleDamage = true;

    public HealthBar healthBar;
    public ManaBar manaBar;
    int collisions = 0;

    public bool isPlayerDead = false;

    float horizontalInput;

    float lastSpeed;

    bool wasGrounded;

    float lastVelocity;

    void Start()
    {
        Player_Health = statHealth.Max_Health;

        Player_MaxHealth = statHealth.Max_Health;

        Mana = statMana.Max_Mana;
        MaxMana = statMana.Max_Mana;

        isPlayerDead = false;
    }

    void Awake()
    {
        if (manaBar == null)
        {
            GameObject manaBarObj = GameObject.Find("ManaBar");
            if (manaBarObj != null)
            {
                manaBar = manaBarObj.GetComponent<ManaBar>();
            }

        }

        if (healthBar == null)
        {
            GameObject healthBarObj = GameObject.Find("HealthBar");
            if (healthBarObj != null)
            {
                healthBar = healthBarObj.GetComponent<HealthBar>();
            }

        }
    }

    void Update()
    {
        if (CutsceneManager.cutscenePlaying) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Flip sprite
        if (horizontalInput < 0) spriteRenderer.flipX = true;
        else if (horizontalInput > 0) spriteRenderer.flipX = false;

        count++;

        if (Input.GetKey(KeyCode.Backspace))
        {
            rb.velocity = new Vector3(rb.velocity.x, 9, 0);
        }
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && !jumpState)
        {
            rb.velocity = new Vector3(rb.velocity.x, 10, 0);
            playerAudioSource.PlayOneShot(playerJump);

            Debug.Log("Played jump!");
            jumpState = true;
        }

        if (jumpState == true && isGrounded)
        {
            jumpState = false;
            playerAudioSource.PlayOneShot(playerFall);

            Debug.Log("Played fall!");
        }

        if (Input.GetKey("n"))
        {
            SceneManager.LoadScene(1);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }


        float horX = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        float veloY = rb.velocity.y;
        float verY = Input.GetAxisRaw("Vertical");

        isCasting = GetIsCasting();
        isCastAnimation = playerAttacks.IsCastAnimation();

        if (!isHit && !isCastAnimation && !isCasting)
        {
            if (lastSpeed != horX)
            {
                lastSpeed = horX;
                animator.SetFloat("Speed", lastSpeed);
            }
            if (wasGrounded != isGrounded)
            {
                wasGrounded = isGrounded;
                animator.SetBool("IsGrounded", wasGrounded);
            }
            if (lastVelocity != veloY)
            {
                lastVelocity = veloY;
                animator.SetFloat("VerticalVelocity", lastVelocity);
            }  
        }
    }

    void FixedUpdate()
    {
        // Ruch poziomy
        float targetSpeed = horizontalInput * maxSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate;
        if (isGrounded)
            accelRate = Mathf.Abs(horizontalInput) > 0.01f ? groundAcceleration : groundDeceleration;
        else
            accelRate = Mathf.Abs(horizontalInput) > 0.01f ? airAcceleration : airDeceleration;

        float movement = speedDiff * accelRate * Time.fixedDeltaTime;
        rb.AddForce(Vector2.right * movement, ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.8f)
            {
                SetGrounded(true);
            }
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0) // Normalna wskazuje na górną część obiektu
            {
                // Zastosuj logikę tylko wtedy, gdy gracz dotyka góry platformy
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {

            //canObstacleDamage = false;
        }
        else if (collision.gameObject.CompareTag("NPC"))
        {
            NPCController npcController = collision.gameObject.GetComponent<NPCController>();

            DamagePlayer(npcController.NPC_Damage, DamageType.Magic);
        }
        else if (collision.gameObject.CompareTag("Obstacles"))
        {
            //canObstacleDamage = false;
        }

        if (collision.gameObject.name == "Teren")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Sprawdzamy, czy normalna wskazuje w górę (czyli uderzamy od góry w coś)
                if (contact.normal.y > 0)
                {

                }
            }
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                SetGrounded(true);
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        SetGrounded(false);
    }

    public bool IsFacingRight()
    {
        return spriteRenderer.flipX == false;
    }

    public float GetHealth()
    {
        return Player_Health;
    }

    public void SetMaxHealth()
    {
        Player_Health = Player_MaxHealth;
        healthBar?.UpdateHealthBar();
    }

    public void DamagePlayer(float damage, DamageType iDamageType)
    {
        if (CutsceneManager.cutscenePlaying) return;

        LevelAnalytics.Instance.damageTaken += damage;

        playerAttacks.OnTakeDamage();

        if (iDamageType == DamageType.Physical)
        {
            damage = damage - statDefenses.Armor;
        }
        else if (iDamageType == DamageType.Magic)
        {
            damage = damage - statDefenses.Magic_Resist;
        }
        else
        {
            damage = damage;
        }

        if (damage <= 0) return;

        Player_Health -= damage;

        if (Player_Health <= 0) Player_Health = 0;

        healthBar?.UpdateHealthBar();

        StartCoroutine(HitAnimation());

        LevelAnalytics.Instance.hp_at_death += Player_Health;
        LevelAnalytics.Instance.mana_at_death += Mana;

        if (SceneManager.GetActiveScene().name == "Level5") LevelAnalytics.Instance.boss_attempts += 1;

        playerAudioSource.PlayOneShot(playerHit);

        Debug.Log("Played hit!");

        if (Player_Health <= 0)
        {
            isPlayerDead = true;
            
            StartCoroutine(GameOver());
        }
    }

    public void RegenerateHealth()
    {
        statHealth.StartHealthRegen();
    }

    public void HealPlayer(float heal)
    {
        Player_Health += heal;
        healthBar?.UpdateHealthBar();

        if (Player_Health >= Player_MaxHealth)
        {
            SetMaxHealth();
        }
    }

    IEnumerator HitAnimation()
    {
        isHit = true;
        animator.SetTrigger("Hit");

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

        isHit = false;
    }

    IEnumerator GameOver()
    {

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

        LevelAnalytics.Instance.tryNumber += 1;
        LevelAnalytics.Instance.deaths += 1;
        LevelAnalytics.Instance.posX += transformPlayer.position.x;
        LevelAnalytics.Instance.posY += transformPlayer.position.y;

        if (SceneManager.GetActiveScene().name == "Level5") LevelAnalytics.Instance.boss_hp_left += GameManager.Instance.boss_hp_left;

        Destroy(gameObject, 1f);


        SceneManager.LoadScene("GameOverScene");
    }

    private bool GetIsCasting()
    {
        return playerAttacks.IsCasting();
    }

    public bool CanObstacleDamage()
    {
        return canObstacleDamage;
    }

    public bool IsEnoughMana(float manaCost)
    {
        return Mana >= manaCost;
    }

    public float GetMana()
    {
        return Mana;
    }

    public void SetMaxMana()
    {
        Mana = MaxMana;
    }
    
    public void ReduceMana(float manaCost)
    {
        if (Mana >= manaCost)
        {
            Mana -= manaCost;
            manaBar.UpdateManaBar();
            
            statMana.StartManaRegen();
        }
    }

    public void RegenMana(float manaRegen)
    {
        Mana += manaRegen;
        manaBar?.UpdateManaBar();

        if (Mana >= MaxMana)
        {
            Mana = MaxMana;
            manaBar?.UpdateManaBar();
        }
    }

    public bool GetIsHit()
    {
        return isHit;
    }

    public void SetGrounded(bool state)
    {
        isGrounded = state;

        if (isGrounded)
            col.sharedMaterial = groundedMaterial;
        else
            col.sharedMaterial = airMaterial;
    }
}
