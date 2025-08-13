using UnityEngine;

public class BOSS2 : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float seriousModeSpeed = 4f;

    public AudioClip attackSFX;
    public AudioClip deathSFX;

    private Animator animator;
    private bool isSerious = false;
    private bool isDead = false;
    private bool isRunning = false;

    private float attackCooldown = 2f;
    private float attackTimer = 0f;

    Rigidbody2D rigidbody2d;

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.IsAlive);
        }
    }

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > 3f && IsAlive)
        {
            MoveToPlayer();
        }
        else if (!IsAlive)
        {
            StopRunning();
            Death();
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown && distanceToPlayer <= 3.1f && IsAlive)
        {
            attackTimer = 0f;

            if (isSerious)
                SeriousCombo();
            else
                AttackRandom();
        }
    }

    void MoveToPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        float speed = isSerious ? seriousModeSpeed : moveSpeed;

        transform.position += direction * speed * Time.deltaTime;

        if (!isRunning)
        {
            isRunning = true;
            animator.SetBool("run", true);
        }

        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void StopRunning()
    {
        if (isRunning)
        {
            isRunning = false;
            animator.SetBool("run", false);
        }
    }

    void AttackRandom()
    {
        animator.ResetTrigger("atk1");
        animator.ResetTrigger("atk2");

        int rand = Random.Range(1, 2);
        if (rand == 1)
            animator.SetTrigger("atk1");

        if (attackSFX != null)
            AudioManager.Instance.PlaySFX(attackSFX);
    }

    void SeriousCombo()
    {
        animator.ResetTrigger("atk1");
        animator.ResetTrigger("atk2");

        int rand = Random.Range(1, 4);

        if (rand == 1)
        {
            animator.SetTrigger("atk1");
            Invoke(nameof(TriggerATK2), 0.4f);
        }
        else if (rand == 2)
        {
            animator.SetTrigger("atk2");
        }

        if (attackSFX != null)
            AudioManager.Instance.PlaySFX(attackSFX);
    }

    void TriggerATK2() => animator.SetTrigger("atk2");

    void EnterSeriousMode()
    {
        isSerious = true;
    }

    void Death()
    {
        if (isDead) return;
        isDead = true;

        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        collider.isTrigger = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (deathSFX != null)
            AudioManager.Instance.PlaySFX(deathSFX);
    }

    public void OnHit(int damage, Vector2 KB)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.hit);
        rigidbody2d.linearVelocity = new Vector2(KB.x, rigidbody2d.linearVelocity.y + KB.y);
    }
}
