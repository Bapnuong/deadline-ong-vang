using UnityEngine;

public class Boss3 : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;

    [Header("Dash Settings")]
    public float dashSpeed = 8f;          // tốc độ dash
    public float dashDistance = 6f;       // khoảng cách phát hiện để dash
    public float dashCooldown = 3f;       // thời gian hồi dash
    public float overshootDistance = 2f;  // khoảng cách đi quá player khi dash xuyên

    [Header("Attack Settings")]
    public float attackCooldown = 1f;     // thời gian hồi sau mỗi đòn đánh
    public float attackRange = 2f;        // khoảng cách đánh thường

    private Animator animator;
    private bool isRunning = false;
    private bool isDashing = false;

    private float attackTimer = 0f;
    private float dashTimer = 0f;

    private Rigidbody2D rigidbody2d;

    public bool IsAlive
    {
        get { return animator.GetBool(AnimationStrings.IsAlive); }
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
    {  // Luôn tìm lại player nếu không thấy hoặc player bị disable
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (player == null) return; // Nếu vẫn chưa tìm thấy thì bỏ qua frame này
        if (!IsAlive)
        {
            StopRunning();
            Death();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu đang dash thì không xử lý gì khác
        if (isDashing) return;

        dashTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        // Dash xuyên qua nếu player trong khoảng dash
        if (dashTimer >= dashCooldown && distanceToPlayer <= dashDistance && distanceToPlayer > attackRange)
        {
            dashTimer = 0f;
            StartCoroutine(DashThroughPlayer());
            return;
        }

        // Đánh thường nếu trong tầm
        if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Attack();
            return;
        }

        // Di chuyển bình thường khi ở xa
        if (distanceToPlayer > attackRange)
        {
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (!isRunning)
        {
            isRunning = true;
            animator.SetBool("run", true);
        }

        FlipTowardsPlayer(direction.x);
    }

    void StopRunning()
    {
        if (isRunning)
        {
            isRunning = false;
            animator.SetBool("run", false);
        }
    }

    void FlipTowardsPlayer(float dirX)
    {
        if (dirX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dirX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    System.Collections.IEnumerator DashThroughPlayer()
    {
        isDashing = true;
        animator.SetTrigger("dash");

        yield return new WaitForSeconds(0.15f); // delay trước khi dash

        Vector3 direction = (player.position - transform.position).normalized;
        FlipTowardsPlayer(direction.x);

        // Điểm kết thúc dash = vị trí player + thêm overshootDistance
        Vector3 targetPos = player.position + (direction * overshootDistance);

        float dashTime = Vector2.Distance(transform.position, targetPos) / dashSpeed;
        float timer = 0f;

        while (timer < dashTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Quay mặt lại phía player sau khi dash xuyên qua
        Vector3 backDir = (player.position - transform.position).normalized;
        FlipTowardsPlayer(backDir.x);

        // Đánh ngay sau khi dash
        yield return new WaitForSeconds(0.2f);
        Attack();

        yield return new WaitForSeconds(0.4f);
        isDashing = false;
    }

    void Attack()
    {
        StopRunning();
        animator.SetTrigger("atk1");
    }

    void Death()
    {
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        collider.isTrigger = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void OnHit(int damage, Vector2 KB)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.hit);
        rigidbody2d.linearVelocity = new Vector2(KB.x, rigidbody2d.linearVelocity.y + KB.y);
    }
}
