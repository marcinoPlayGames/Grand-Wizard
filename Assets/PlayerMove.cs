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
    void Start()
    {
        Debug.Log("Start in PlayerMovement");
        rb = GetComponent<Rigidbody2D>();
        Player_Health = Player_MaxHealth;
        Debug.Log(healthBar.healthBarImage.fillAmount);
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

    public HealthBar healthBar;

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
            rb.velocity = new Vector3(rb.velocity.x, 9, 0);
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


        if (!isHit)
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
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;

        if (collision.gameObject.CompareTag("NPC"))
        {
            NPCController npcController = collision.gameObject.GetComponent<NPCController>();

            DamagePlayer(npcController.NPC_Damage);
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
    public void DamagePlayer(float damage)
    {
        Debug.Log(Player_Health);

        Player_Health -= damage;
        healthBar.UpdateHealthBar();
        StartCoroutine(HitAnimation());

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
}
