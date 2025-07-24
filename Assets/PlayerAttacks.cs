using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    public float fireballSpeed = 5f;
    public GameObject fireballPrefab;
    private bool isFacingRight = true;

    private GameObject lastSpawnedFireball;
    private GameObject lastSpawnedStrongerFireball;

    private bool canCastFireball = true;
    public float fireballCooldown = 1.5f; // Adjust the cooldown duration as needed
    public float fireballDamage = 50;

    public float strongerFireballDamage = 200;
    public float strongerFireballCooldown = 2.5f;
    public float strongerFireballSpeed = 10f;
    public GameObject strongerFireballPrefab;

    private bool canCastStrongerFireball = true;

    private PlayerMove playerMove;

    bool isCasting = false;
    bool hasPulled = false;
    bool isHiding = false;
    bool isPulling = false;
    bool isCastAnimation = false;

    private Coroutine castCoroutine;

    [SerializeField]
    AudioSource Fireball;

    [SerializeField]
    AudioSource StrongerFireball;

    private StatAttacks statAttacks;
    private StatCriticals statCriticals;
    private StatHealth statHealth;

    private bool isEnoughMana = true;

    // Start is called before the first frame update
    void Start()
    {
        // Find the PlayerMove script attached to the same GameObject
        
        
        playerMove = GetComponent<PlayerMove>();

        statAttacks = GetComponent<StatAttacks>();
        statCriticals = GetComponent<StatCriticals>();
        statHealth = GetComponent<StatHealth>();

        fireballSpeed = fireballSpeed; // * (1 + statAttacks.Attack_Speed);
        strongerFireballSpeed = strongerFireballSpeed; // * (1 + statAttacks.Spell_Speed);
    }

    // Update is called once per frame

    void Update()
    {

        // Check if the "1" key is pressed and the player can cast a fireball

        if (Input.GetKeyDown(KeyCode.Alpha1) && canCastFireball)
        {
            isEnoughMana = playerMove.IsEnoughMana(WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Base", false));

            Debug.Log("Mana needed = " + WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Base", false));
            Debug.Log("IsEnoughMana = " + isEnoughMana);

            if (!hasPulled && !isPulling)
            {
                StartCoroutine(PullStaffAnimation());
            }
            else if (hasPulled && !isPulling && !isCasting && isEnoughMana)
            {
                StartCoroutine(CastFireballAnimation());
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && canCastStrongerFireball)
        {
            isEnoughMana = playerMove.IsEnoughMana(WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Ability", false));

            Debug.Log("Mana needed = " + WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Ability", false));
            Debug.Log("IsEnoughMana = " + isEnoughMana);

            if (!hasPulled && !isPulling)
            {
                StartCoroutine(PullStaffAnimation());
            }
            else if (hasPulled && !isPulling && !isCasting && isEnoughMana)
            {
                StartCoroutine(CastStrongerFireballAnimation());
            }
        }

        /*if (lastSpawnedFireball != null)
        {
            Vector2 rayOrigin = lastSpawnedFireball.transform.position; // Use player's position as the origin
            Vector2 rayDirection = isFacingRight ? lastSpawnedFireball.transform.right : -lastSpawnedFireball.transform.right;

            // Use a LayerMask to ignore the "Fireball" layer
            LayerMask layerMask = ~LayerMask.GetMask("Fireball");
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 0.2f, layerMask);

            // Debugging information
            Debug.DrawRay(rayOrigin, rayDirection * 0.2f, Color.red); // Draw the ray in the Scene view

            if (hit.collider != null)
            {
                // Destroy the fireball if it hits terrain or an NPC
                Destroy(lastSpawnedFireball);
                Debug.Log("Fireball Collided with: " + hit.collider.gameObject.name);
            }

            if (hit.collider != null && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("NPC")))
            {
                // Destroy the fireball if it hits terrain or an NPC
                Destroy(lastSpawnedFireball);
                Debug.Log("Fireball Collided with: " + hit.collider.gameObject.name);
            }
        }*/
    }

    IEnumerator SpawnAndShootFireball()
    {
        Debug.Log("Fire casted!");
        isFacingRight = playerMove.IsFacingRight();
        Vector2 fireballDirection = isFacingRight ? Vector2.right : Vector2.left;
        float spawnOffset = isFacingRight ? 2f : -2f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.3f, 0f);

        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
        Fireball.Play();

        yield return null;

        if (fireball != null)
        {
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.velocity = fireballDirection * fireballSpeed;
            }

            float totalDamage = WeaponUpgradeSystem.Instance.GetTotalDamage("Staff", false);
            bool GotCrit = statCriticals.IsAttackCriticalHit();

            Debug.Log("Physical Damage = " + totalDamage);

            if (GotCrit)
            {
                Debug.Log("Got Physical Crit!");
                totalDamage = statCriticals.GetCriticalDamageByAttackType(totalDamage, "Physical");
            }

            Debug.Log("Critical Physical Damage = " + totalDamage);

            Debug.Log("Weaker damage = " + totalDamage);

            statHealth.HealFromDamage(totalDamage);

            float healValue = statHealth.GetHealingValue(totalDamage);

            Debug.Log("Weaker healing = " + healValue);

            float manaCost = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Base", false);
            playerMove.ReduceMana(manaCost);

            FireballController controller = fireball.GetComponent<FireballController>();
            if (controller != null)
                controller.SetDamage(totalDamage);

            Destroy(fireball, 2f);
        }

        StartCoroutine(FireballCooldown());
    }

    IEnumerator SpawnAndShootStrongerFireball()
    {
        Debug.Log("Fire casted!");
        isFacingRight = playerMove.IsFacingRight();
        Vector2 fireballDirection = isFacingRight ? Vector2.right : Vector2.left;
        float spawnOffset = isFacingRight ? 2f : -2f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.3f, 0f);
        Vector2 fireballFacing = isFacingRight ? Vector2.right : Vector2.left;

        GameObject strongerFireball = Instantiate(strongerFireballPrefab, spawnPosition, Quaternion.identity);
        Fireball.Play();

        yield return null;

        if (strongerFireball != null)
        {
            Rigidbody2D rb = strongerFireball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.velocity = fireballDirection * strongerFireballSpeed;
            }

            float totalDamage = 2 * WeaponUpgradeSystem.Instance.GetTotalDamage("Staff", false);

            bool GotCrit = statCriticals.IsSpellCriticalHit();

            Debug.Log("Magic Damage = " + totalDamage);

            if (GotCrit)
            {
                Debug.Log("Got Magic Crit!");
                totalDamage = statCriticals.GetCriticalDamageByAttackType(totalDamage, "Ability");
            }

            Debug.Log("Critical Magic Damage = " + totalDamage);

            Debug.Log("Stronger damage = " + totalDamage);

            statHealth.HealFromDamage(totalDamage);

            float healValue = statHealth.GetHealingValue(totalDamage);

            Debug.Log("Stronger healing = " + healValue);

            float manaCost = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Ability", false);

            playerMove.ReduceMana(manaCost);

            FireballController controller = strongerFireball.GetComponent<FireballController>();
            if (controller != null)
                controller.SetDamage(totalDamage);

            if (fireballFacing == new Vector2(-1.00f, 0.00f))
                strongerFireball.GetComponent<SpriteRenderer>().flipX = true;
            else
                strongerFireball.GetComponent<SpriteRenderer>().flipX = false;

            Destroy(strongerFireball, 2f);
        }

        StartCoroutine(StrongerFireballCooldown());
    }

    /*void CastStrongerFireball()
    {
        // Determine the direction to cast the fireball
        isFacingRight = playerMove.IsFacingRight();
        Vector2 fireballDirection = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 fireballFacing = isFacingRight ? Vector2.right : Vector2.left;

        Debug.Log("fireballFacing = " + fireballFacing);

        // Adjust the instantiation position based on the player's facing direction
        float spawnOffset = isFacingRight ? 2f : -2f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset, 0.8f, 0f);

        // Create a new fireball instance using the actual fireball prefab
        lastSpawnedStrongerFireball = Instantiate(strongerFireballPrefab, spawnPosition, Quaternion.identity);

        StrongerFireball.Play();

        // Set the fireball's velocity based on the direction and speed
        Rigidbody2D fireballRb = lastSpawnedStrongerFireball.GetComponent<Rigidbody2D>();
        fireballRb.velocity = fireballDirection * fireballSpeed;
        fireballRb.gravityScale = 0f;
        FireballController fireballController = lastSpawnedStrongerFireball.GetComponent<FireballController>();

        if (fireballFacing == new Vector2(-1.00f, 0.00f))
            lastSpawnedStrongerFireball.GetComponent<SpriteRenderer>().flipX = true;
        else
            lastSpawnedStrongerFireball.GetComponent<SpriteRenderer>().flipX = false;
        // Destroy the fireball after a certain time to prevent cluttering the scene
        if (fireballController != null)
        {
            fireballController.SetDamage(strongerFireballDamage);
        }

        Destroy(lastSpawnedStrongerFireball, 2f);

        StartCoroutine(StrongerFireballCooldown());
    }*/

    IEnumerator FireballCooldown()
    {
        // Set canCastFireball to false during the cooldown
        canCastFireball = false;

        fireballCooldown = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Attack_Speed", false);

        Debug.Log("Attack_Speed = " + fireballCooldown);
        // Wait for the cooldown duration
        yield return new WaitForSeconds(fireballCooldown);

        // Set canCastFireball to true to allow casting again
        canCastFireball = true;
    }

    IEnumerator PullStaffAnimation()
    {
        isPulling = true;

        Debug.Log("Staff pulled!");
        GetComponent<Animator>().SetInteger("moveState", 6);


        // Wait for the cooldown duration

        yield return new WaitForSeconds(9f / 10f);

        StartCoroutine(CastFireballsAnimationCooldown());

        hasPulled = true;
        isPulling = false;
    }

    IEnumerator CastFireballAnimation()
    {
        isCasting = true;

        GetComponent<Animator>().SetTrigger("PlayerCastFireballs");

        // Wait for the cooldown duration
        yield return new WaitForSeconds(5f / 20f);

        Debug.Log(IsCastAnimation());
        isCasting = false;
        
        StartCoroutine(SpawnAndShootFireball());
    }

    IEnumerator CastStrongerFireballAnimation()
    {
        isCasting = true;


        GetComponent<Animator>().SetTrigger("PlayerCastFireballs");

        // Wait for the cooldown duration
        yield return new WaitForSeconds(5f / 20f);

        Debug.Log(IsCastAnimation());
        isCasting = false;

        StartCoroutine(SpawnAndShootStrongerFireball());
    }

    IEnumerator CastFireballsAnimationCooldown()
    {
        float cooldownTime = 5f; // Czas oczekiwania przed możliwością ponownego strzału
        float timeRemaining = cooldownTime;
        Debug.Log("Cooldown animation!");

        while (timeRemaining > 0)
        {
            yield return null; // Czekaj do następnej klatki

            // Zmniejsz czas oczekiwania
            timeRemaining -= Time.deltaTime;

            // Sprawdź, czy strzał został oddany (reset czasu)
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Resetuj czas oczekiwania
                timeRemaining = cooldownTime;
                Debug.Log("Czas oczekiwania zresetowany!");
            }
        }

        Debug.Log("Możesz ponownie strzelić!");
        StartCoroutine(HideStaffAnimation());
    }

    IEnumerator HideStaffAnimation()
    {
        isHiding = true;

        Debug.Log("Staff hidden!");
        GetComponent<Animator>().SetInteger("moveState", 7);

        yield return new WaitForSeconds(9f / 10f);

        isHiding = false;
        hasPulled = false;
    }

    public float GetFireballDamage()
    {
        return fireballDamage;
    }

    IEnumerator StrongerFireballCooldown()
    {
        // Set canCastFireball to false during the cooldown
        canCastStrongerFireball = false;

        strongerFireballCooldown = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Spell_Speed", false);

        Debug.Log("Spell_Speed = " + strongerFireballCooldown);

        // Wait for the cooldown duration
        yield return new WaitForSeconds(strongerFireballCooldown);

        // Set canCastFireball to true to allow casting again
        canCastStrongerFireball = true;
    }

    public float GetStrongerFireballDamage()
    {
        return strongerFireballDamage;
    }

    public bool IsCasting()
    {
        return isCasting;
    }

    public bool IsCastAnimation()
    {
        if (!isPulling && !isCasting && !isHiding) return isCastAnimation = false;
        else return true;
    }
}
