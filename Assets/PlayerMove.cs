using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody2D rb;
    private PlayerAttacks playerAttacks;
    private StatHealth statHealth;
    private StatDefenses statDefenses;
    private DamageType damageType;
    void Start()
    {
        Debug.Log("Start in PlayerMovement");
        rb = GetComponent<Rigidbody2D>();

        statHealth = GetComponent<StatHealth>();
        statDefenses = GetComponent<StatDefenses>();
        Player_Health = statHealth.Max_Health;

        Player_MaxHealth = statHealth.Max_Health;
        Debug.Log(healthBar.healthBarImage.fillAmount);

        playerAttacks = GetComponent<PlayerAttacks>();

        Debug.Log("StatSystem.Instance = " + StatSystem.Instance);
        Debug.Log("Max HP = " + Player_Health);
    }

    [SerializeField]
    AudioSource playerJump;

    [SerializeField]
    AudioSource playerFall;

    [SerializeField]
    AudioSource playerHit;


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
    int collisions = 0;

    void Update()
    {
        //Debug.Log("called Update " + count.ToString());
        count++;

        if (Input.GetKey(KeyCode.Backspace))
        {
            rb.velocity = new Vector3(rb.velocity.x, 9, 0);
        }
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space)) && !jumpState)
        {
            rb.velocity = new Vector3(rb.velocity.x, 10, 0);
            playerJump.Play();
            jumpState = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {

            float newYVelocity = rb.velocity.y;

            rb.velocity = new Vector3(-4, newYVelocity, 0);


        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float newYVelocity = rb.velocity.y;

            Debug.Log(newYVelocity);

            rb.velocity = new Vector3(4, newYVelocity, 0);
        }

        if (jumpState == true && isGrounded)
        {
            jumpState = false;
            playerFall.Play();
        }

        if (Input.GetKey("n"))
        {
            SceneManager.LoadScene(1);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }


        float horX = Input.GetAxisRaw("Horizontal");
        float veloY = rb.velocity.y;
        float verY = Input.GetAxisRaw("Vertical");

        //Debug.Log($"horY = {rb.velocity.y}, horX = {rb.velocity.x}");
        //Debug.Log($"positionY = {rb.position.y}, horY = {Input.GetAxisRaw("Vertical")}");
        //Debug.Log($"Is grounded? {isGrounded}");
        isCasting = GetIsCasting();
        isCastAnimation = playerAttacks.IsCastAnimation();

        if (!isHit && !isCastAnimation && !isCasting)
        {
            if (!isGrounded && veloY > 0) // If moving upward (jumping)
            {
                GetComponent<Animator>().SetInteger("moveState", 2); // Set jump animation
            }
            else if (!isGrounded && veloY < 0)
            {
                GetComponent<Animator>().SetInteger("moveState", 3); // Set fall animation
            }
            else if (isGrounded && horX > 0)
            {
                GetComponent<Animator>().SetInteger("moveState", 1); // Set run animation
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (isGrounded && horX < 0)
            {
                GetComponent<Animator>().SetInteger("moveState", 1); // Set run animation
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<Animator>().SetInteger("moveState", 0); // Set idle animation
            }
            //Debug.Log("isCastAnimation: " + isCastAnimation);  // Dodaj to do debugowania
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;

        //Debug.Log($"Collided with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        Debug.Log($"Collided with: {collision.gameObject.name}, Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0) // Normalna wskazuje na górną część obiektu
            {
                Debug.Log("Collided with top side of the platform!");
                // Zastosuj logikę tylko wtedy, gdy gracz dotyka góry platformy
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Collided with Wall!");

            //canObstacleDamage = false;
        }
        else if (collision.gameObject.CompareTag("NPC"))
        {
            NPCController npcController = collision.gameObject.GetComponent<NPCController>();

            DamagePlayer(npcController.NPC_Damage, DamageType.Magic);
        }
        else if (collision.gameObject.CompareTag("Obstacles"))
        {
            Debug.Log("Collided with Obstacles!");

            //canObstacleDamage = false;
        }

        Debug.Log("Collidian = " + collision.gameObject.name);
        if (collision.gameObject.name == "Teren")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Sprawdzamy, czy normalna wskazuje w górę (czyli uderzamy od góry w coś)
                if (contact.normal.y > 0)
                {
                    Debug.Log("Stoi na czymś (np. na terenie)");
                }
            }
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = true;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public bool IsFacingRight()
    {
        return GetComponent<SpriteRenderer>().flipX == false;
    }

    public float GetHealth()
    {
        return Player_Health;
    }
    public void DamagePlayer(float damage, DamageType iDamageType)
    {
        Debug.Log(Player_Health);

        Debug.Log("Player take damage original = " + damage);

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

        Player_Health -= damage;
        healthBar.UpdateHealthBar();
        Debug.Log("Damage = " + damage);
        StartCoroutine(HitAnimation());

        Debug.Log("Player take damage defended = " + damage);

        playerHit.Play();

        Debug.Log(Player_Health);

        if (Player_Health <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    IEnumerator HitAnimation()
    {
        isHit = true;
        GetComponent<Animator>().SetInteger("moveState", 4);

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

        isHit = false;
    }

    IEnumerator GameOver()
    {

        // Wait for the cooldown duration
        yield return new WaitForSeconds(1f);

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
}
