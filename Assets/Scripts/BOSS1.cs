    using UnityEngine;

    public class BOSS1 : MonoBehaviour
    {
        public Transform player;
        public float moveSpeed = 2f;
        public float seriousModeSpeed = 4f;

        [Header("Dash Skill")]
        public float dashSpeed = 8f;
        public float dashDelay = 0.3f;
        public int dashCount = 3;
        public float dashCooldown = 6f;
        private float dashTimer = 0f;

        private Animator animator;
        private bool isSerious = false;
        private bool isDead = false;
        private bool isRunning = false;
        private bool isDashing = false;

        private float attackCooldown = 2f;
        private float attackTimer = 0f;

        private Rigidbody2D rigidbody2d;

        public bool IsAlive => animator.GetBool(AnimationStrings.IsAlive);

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
                if (p != null) player = p.transform;
            }
        }

        void Update()
        {
            if (player == null || !player.gameObject.activeInHierarchy)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            if (player == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!isDashing)
            {
                if (distanceToPlayer > 2f && IsAlive)
                {
                    MoveToPlayer();
                }
                else if (!IsAlive)
                {
                    StopRunning();
                    Death();
                }
            }

            attackTimer += Time.deltaTime;
            dashTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown && distanceToPlayer <= 2.1f && IsAlive && !isDashing)
            {
                attackTimer = 0f;

                // Nếu đang ở serious mode và dash cooldown xong thì dùng skill lướt
                if (isSerious && dashTimer >= dashCooldown)
                {
                    dashTimer = 0f;
                    StartCoroutine(DashAttack());
                }
                else
                {
                    if (isSerious) SeriousCombo();
                    else AttackRandom();
                }
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
                animator.SetBool("Run", true);
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
                animator.SetBool("Run", false);
            }
        }

        void AttackRandom()
        {
            animator.ResetTrigger("ATK1");
            animator.ResetTrigger("ATK2");

            int rand = Random.Range(1, 3);
            animator.SetTrigger(rand == 1 ? "ATK1" : "ATK2");

            AudioManager.Instance.PlaySFX(AudioManager.Instance.bossAttack);
        }

        void SeriousCombo()
        {
            animator.ResetTrigger("ATK1");
            animator.ResetTrigger("ATK2");
            animator.ResetTrigger("ATK3");

            int rand = Random.Range(1, 4);

            if (rand == 1)
            {
                animator.SetTrigger("ATK1");
                Invoke(nameof(TriggerATK2), 0.4f);
            }
            else if (rand == 2)
            {
                animator.SetTrigger("ATK2");
                Invoke(nameof(TriggerATK3), 0.5f);
            }
            else
            {
                animator.SetTrigger("ATK1");
                Invoke(nameof(TriggerATK2), 0.4f);
                Invoke(nameof(TriggerATK3), 0.8f);
            }

            AudioManager.Instance.PlaySFX(AudioManager.Instance.bossAttack);
        }

        System.Collections.IEnumerator DashAttack()
        {
            isDashing = true;
            StopRunning();
            animator.SetTrigger("DashSkill");

            yield return new WaitForSeconds(0.3f); // thời gian chuẩn bị trước khi dash

            for (int i = 0; i < dashCount; i++)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                rigidbody2d.linearVelocity = dir * dashSpeed;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.bossAttack);
                yield return new WaitForSeconds(dashDelay);
                rigidbody2d.linearVelocity = Vector2.zero;
                yield return new WaitForSeconds(0.1f);
            }

            isDashing = false;
        }

        void TriggerATK2() => animator.SetTrigger("ATK2");
        void TriggerATK3() => animator.SetTrigger("ATK3");

        void EnterSeriousMode() => isSerious = true;

        void Death()
        {
            if (isDead) return;
            isDead = true;

            GetComponent<CapsuleCollider2D>().isTrigger = true;
            rigidbody2d.linearVelocity = Vector2.zero;
            rigidbody2d.bodyType = RigidbodyType2D.Kinematic;

            AudioManager.Instance.PlaySFX(AudioManager.Instance.bossDeath);
        }

        public void OnHit(int damage, Vector2 KB)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.hit);
            rigidbody2d.linearVelocity = new Vector2(KB.x, rigidbody2d.linearVelocity.y + KB.y);
        }
    }
