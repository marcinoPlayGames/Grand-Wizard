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

    float regenDelay = 3f;
    float timeSinceLastDamage = 0f;
    bool isInCombat = false;

    [SerializeField]
    private PlayerMove playerMove;

    bool isCasting = false;
    bool hasPulled = false;
    bool isHiding = false;
    bool isPulling = false;
    bool isCastAnimation = false;

    private Coroutine castCoroutine;

    [SerializeField]
    AudioSource playerAudioSource;

    [SerializeField]
    AudioClip Fireball;

    [SerializeField]
    AudioClip StrongerFireball;

    [SerializeField]
    AudioClip CantCast;

    [SerializeField]
    AudioClip CantAttack;

    [SerializeField]
    AudioClip NotEnoughMana;

    [SerializeField]
    AudioClip HidePullStaff;

    [SerializeField]
    private StatAttacks statAttacks;
    [SerializeField]
    private StatCriticals statCriticals;
    [SerializeField]
    private StatHealth statHealth;
    [SerializeField]
    private Animator animator;

    private bool isEnoughMana = true;

    CooldownDisplay normalAttackCooldownDisplay;
    CooldownDisplay strongerAttackCooldownDisplay;

    [SerializeField]
    LayerMask fireballSpawnMask;

    private bool strongerFireballShoot = false;

    float manaBase;
    float manaAbility;

    float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        // Find the PlayerMove script attached to the same GameObject

        fireballSpeed = fireballSpeed; // * (1 + statAttacks.Attack_Speed);
        strongerFireballSpeed = strongerFireballSpeed; // * (1 + statAttacks.Spell_Speed);

        StartCoroutine(PullStaffAnimation());
        playerAudioSource.PlayOneShot(HidePullStaff);

        manaBase = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Base", false);
        attackRange = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Attack_Range", false);
        manaAbility = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Ability", false);
    }

    void Awake()
    {
        if (normalAttackCooldownDisplay == null)
        {
            GameObject normalAttackCooldownDisplayObj = GameObject.Find("NormalAttackCooldown");
            if (normalAttackCooldownDisplayObj != null)
            {
                normalAttackCooldownDisplay = normalAttackCooldownDisplayObj.GetComponent<CooldownDisplay>();
            }

        }

        if (strongerAttackCooldownDisplay == null)
        {
            GameObject strongerAttackCooldownDisplayObj = GameObject.Find("StrongerAttackCooldown");
            if (strongerAttackCooldownDisplayObj != null)
            {
                strongerAttackCooldownDisplay = strongerAttackCooldownDisplayObj.GetComponent<CooldownDisplay>();
            }

        }
    }

    // Update is called once per frame

    void Update()
    {
        if (CutsceneManager.cutscenePlaying) return;
        if (GameManager.Instance.gamePaused) return;

        // Check if the "1" key is pressed and the player can cast a fireball

        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage >= regenDelay && !isInCombat)
        {
            playerMove.RegenerateHealth();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!hasPulled && !isPulling && !playerMove.GetIsHit())
            {
                animator.SetTrigger("PullStaff");
                PullStaffAnimation();
                playerAudioSource.PlayOneShot(HidePullStaff);
            }
            else if (hasPulled && !isPulling && !isHiding && !playerMove.GetIsHit())
            {
                animator.SetTrigger("HideStaff");
                HideStaffAnimation();
                playerAudioSource.PlayOneShot(HidePullStaff);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (canCastFireball)
            {
                isEnoughMana = playerMove.IsEnoughMana(WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Base", false));

                if (hasPulled && !isPulling && !isCasting && isEnoughMana && !playerMove.GetIsHit())
                {
                    CastFireballAnimation();
                }
                else if (!isEnoughMana)
                {
                    playerAudioSource.PlayOneShot(NotEnoughMana);
                }
            }
            else
            {
                playerAudioSource.PlayOneShot(CantAttack);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (canCastStrongerFireball)
            {
                isEnoughMana = playerMove.IsEnoughMana(WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Mana_Ability", false));

                if (hasPulled && !isPulling && !isCasting && isEnoughMana && !playerMove.GetIsHit())
                {
                    CastStrongerFireballAnimation();
                }
                else if (!isEnoughMana)
                {
                    playerAudioSource.PlayOneShot(NotEnoughMana);
                }
            }
            else
            {
                playerAudioSource.PlayOneShot(CantCast);
            }
        }
    }

    IEnumerator SpawnAndShootFireball()
    {
        LevelAnalytics.Instance.attacks_in_level++;

        Debug.Log("Spawned fireball!");

        bool isFacingRight = playerMove.IsFacingRight();
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

        float maxSpawnDistance = 2f;
        float safeMargin = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            maxSpawnDistance,
            fireballSpawnMask
        );

        float spawnDistance = hit.collider != null
            ? Mathf.Max(hit.distance - safeMargin, 0.3f)
            : maxSpawnDistance;

        Vector3 spawnPosition =
            transform.position +
            (Vector3)(dir * spawnDistance) +
            new Vector3(0f, 0.3f, 0f);

        // 🔁 POOL
        GameObject fireball = FireballPool.Instance.GetFireball(FireballType.Normal);
        fireball.transform.position = spawnPosition;
        fireball.transform.rotation = Quaternion.identity;

        playerAudioSource.PlayOneShot(Fireball);

        yield return null; // zostawiamy – fizyka zaskoczy poprawnie

        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = dir * fireballSpeed;

        float totalDamage = WeaponUpgradeSystem.Instance.GetTotalDamage("Staff", false);

        if (statCriticals.IsAttackCriticalHit())
        {
            totalDamage = statCriticals.GetCriticalDamageByAttackType(
                totalDamage, "Physical"
            );
        }

        statHealth.HealFromDamage(totalDamage);
        playerMove.ReduceMana(manaBase);

        FireballController controller = fireball.GetComponent<FireballController>();
        controller.SetDamage(totalDamage);
        controller.SetTracking(spawnPosition, attackRange);

        StartCoroutine(FireballCooldown());
    }

    IEnumerator SpawnAndShootStrongerFireball()
    {
        LevelAnalytics.Instance.abilities_in_level++;

        Debug.Log("Spawned stronger fireball!");

        bool isFacingRight = playerMove.IsFacingRight();
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

        float maxSpawnDistance = 2f;
        float safeMargin = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            maxSpawnDistance,
            fireballSpawnMask
        );

        float spawnDistance = hit.collider != null
            ? Mathf.Max(hit.distance - safeMargin, 0.3f)
            : maxSpawnDistance;

        Vector3 spawnPosition =
            transform.position +
            (Vector3)(dir * spawnDistance) +
            new Vector3(0f, 0.3f, 0f);

        // 🔁 POOL
        GameObject strongerFireball = FireballPool.Instance.GetFireball(FireballType.Strong);
        strongerFireball.transform.position = spawnPosition;
        strongerFireball.transform.rotation = Quaternion.identity;

        playerAudioSource.PlayOneShot(Fireball);

        yield return null;

        Rigidbody2D rb = strongerFireball.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = dir * strongerFireballSpeed;

        float totalDamage = 2f * WeaponUpgradeSystem.Instance.GetTotalDamage("Staff", false);

        if (statCriticals.IsSpellCriticalHit())
        {
            totalDamage = statCriticals.GetCriticalDamageByAttackType(
                totalDamage, "Ability"
            );
        }

        statHealth.HealFromDamage(totalDamage);
        playerMove.ReduceMana(manaAbility);

        FireballController controller = strongerFireball.GetComponent<FireballController>();
        controller.SetDamage(totalDamage);
        controller.SetTracking(spawnPosition, attackRange);

        SpriteRenderer sr = strongerFireball.GetComponent<SpriteRenderer>();
        sr.flipX = !isFacingRight;

        StartCoroutine(StrongerFireballCooldown());
    }

    IEnumerator FireballCooldown()
    {
        // Set canCastFireball to false during the cooldown
        canCastFireball = false;

        fireballCooldown = WeaponUpgradeSystem.Instance.GetWeaponCostsCalculated("Staff", "Attack_Speed", false);

        normalAttackCooldownDisplay.StartUIAttackCooldown(fireballCooldown);

        // Wait for the cooldown duration
        yield return new WaitForSeconds(fireballCooldown);

        // Set canCastFireball to true to allow casting again
        canCastFireball = true;
    }

    IEnumerator PullStaffAnimation()
    {
        isPulling = true;

        animator.SetBool("HasStaff", true);

        //animator.SetTrigger("PullStaff");

        animator.SetTrigger("OpenCombat");

        // Wait for the cooldown duration

        yield return new WaitForSeconds(9f / 10f);

        StartCoroutine(CastFireballsAnimationCooldown());

        hasPulled = true;
        isPulling = false;
    }

    void CastFireballAnimation()
    {
        isCasting = true;

        Debug.Log("Casting fireball!");

        animator.SetBool("IsCasting", true);

        strongerFireballShoot = false;   
    }

    public void OnFireballSpawn()
    {
        if (!isCasting) return;
        
        isCasting = false;

        animator.SetBool("IsCasting", false);

        if (strongerFireballShoot) StartCoroutine(SpawnAndShootStrongerFireball());
        else
        {
            StartCoroutine(SpawnAndShootFireball());
        }        
    }

    void CastStrongerFireballAnimation()
    {
        isCasting = true;

        animator.SetBool("IsCasting", true);

        Debug.Log("Casting stronger fireball!");

        strongerFireballShoot = true;
    }

    IEnumerator CastFireballsAnimationCooldown()
    {
        float cooldownTime = 5f; // Czas oczekiwania przed możliwością ponownego strzału
        float timeRemaining = cooldownTime;

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
            }
        }
    }

    IEnumerator HideStaffAnimation()
    {
        isHiding = true;

        animator.SetBool("HasStaff", false);

        animator.SetTrigger("OpenCombat");

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

        strongerAttackCooldownDisplay.StartUIAttackCooldown(strongerFireballCooldown);

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

    public void OnTakeDamage()
    {
        timeSinceLastDamage = 0f;
        isInCombat = true;

        // FAIL-SAFE: Jeśli oberwaliśmy, przerywamy rzucanie czaru
        isCasting = false;
        strongerFireballShoot = false;

        Invoke("ExitCombat", 5f);
    }

    void ExitCombat()
    {
        isInCombat = false;
    }
}
