using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;

    [SerializeField] private float MoveSpeed = 3f;
    [SerializeField] private float jumpImpulse = 10f;
    [SerializeField] private float AirWalkSpeed = 3f;
    [SerializeField] private float DashSPD = 20f;
    [SerializeField] private float DashDuration = 0.1f;
    [SerializeField] private float DashCD = 0.1f;

    [SerializeField] private bool _isDashing;
    public bool CanDash = true;
    TrailRenderer trailRenderer;

    // footstep sound control
    private float footstepTimer;
    public float footstepInterval = 0.3f;

    public bool IsAlive => animator.GetBool(AnimationStrings.IsAlive);

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove && !IsCrouching)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    return touchingDirections.IsGrounded ? MoveSpeed : AirWalkSpeed;
                }
            }
            return 0;
        }
    }

    [SerializeField] private bool _ismoving = false;
    public bool IsMoving
    {
        get => _ismoving;
        private set
        {
            _ismoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField] private bool _iscrouching = false;
    public bool IsCrouching
    {
        get => _iscrouching;
        set
        {
            _iscrouching = value;
            animator.SetBool(AnimationStrings.isCrouching, value);
        }
    }

    [SerializeField] private bool _isattacking = false;
    public bool IsAttacking
    {
        get => _isattacking;
        set => _isattacking = value;
    }

    private bool _isfacingright = true;
    public bool IsFacingRight
    {
        get => _isfacingright;
        set
        {
            if (_isfacingright != value)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (value ? 1 : -1);
                transform.localScale = scale;
            }
            _isfacingright = value;
        }
    }

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    public bool LockVelocity => animator.GetBool(AnimationStrings.LockVelocity);

    private Rigidbody2D rb;
    private Animator animator;
    private TouchingDirections touchingDirections;
    private AudioManager audioManager;
    private DamageAble damageAble;

    private bool wasGroundedLastFrame = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        audioManager = UnityEngine.Object.FindFirstObjectByType<AudioManager>();
        damageAble = GetComponent<DamageAble>();
    }

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void FixedUpdate()
    {
        if (_isDashing) return;

        if (!LockVelocity)
        {
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);

        if (IsAlive && CanMove && moveInput.x != 0)
        {
            SetFacingDirection(moveInput);
        }

        // Footstep sound khi đi bộ trên mặt đất (thay run -> walk)
        if (IsAlive && CanMove && touchingDirections.IsGrounded && IsMoving && !_isDashing)
        {
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0f)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.walk);
                footstepTimer = footstepInterval;
            }
        }

        // Land SFX khi vừa tiếp đất
        if (touchingDirections.IsGrounded && !wasGroundedLastFrame)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.land);
        }
        wasGroundedLastFrame = touchingDirections.IsGrounded;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
        }
        else
        {
            IsMoving = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && CanDash)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.dash);
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(6, 8, true);
        CanDash = false;
        _isDashing = true;
        trailRenderer.emitting = true;

        float dashDirection = IsFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * DashSPD, rb.linearVelocity.y);

        yield return new WaitForSeconds(DashDuration);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        _isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(6, 8, false);

        yield return new WaitForSeconds(DashCD);
        CanDash = true;
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight) IsFacingRight = true;
        else if (moveInput.x < 0 && IsFacingRight) IsFacingRight = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started) IsCrouching = true;
        else if (context.canceled) IsCrouching = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.JumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

            AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !IsAttacking)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.punch);
            IsAttacking = true;

            if (IsCrouching && touchingDirections.IsGrounded)
                animator.SetTrigger(AnimationStrings.CrouchAttackTrigger);
            else
                animator.SetTrigger(AnimationStrings.AttackTrigger);
        }
        else if (context.canceled)
        {
            IsAttacking = false;
        }
    }

    public void OnKick(InputAction.CallbackContext context)
    {
        if (context.started && !IsAttacking && touchingDirections.IsGrounded)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.kick);
            IsAttacking = true;
            animator.SetTrigger(AnimationStrings.KickTrigger);
        }
        else if (context.canceled)
        {
            IsAttacking = false;
        }
    }

    public void Death()
    {
        if (!IsAlive)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.death);

            CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
            collider.isTrigger = true;

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            animator.SetTrigger(AnimationStrings.DeathTrigger);
        }
    }

    public void OnHit(int damage, Vector2 KB)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.hurt);
        rb.linearVelocity = new Vector2(KB.x, rb.linearVelocity.y + KB.y);
    }
}
