using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class DamageAble : MonoBehaviour
{
    public UnityEvent<int,Vector2> damageableHit;
    public UnityEvent onDeath;
    [SerializeField] private int _maxhealth = 100;
    public int MaxHealth
    {
        get
        {
            return _maxhealth;
        }
        set
        {
            _maxhealth = value;
            
        }
    }

    [SerializeField] private int _health = 100;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
                onDeath?.Invoke();
            }
        }
    }

    [SerializeField] private bool _isAlive = true;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.IsAlive, value);
        }
    }

    Animator animator;

    [SerializeField] private bool IsInvincible = false;
    private float timesinceHit;
    public float IsInvincibletime = 0.25f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsInvincible)
        {
            if(timesinceHit > IsInvincibletime)
            {
                IsInvincible = false;
                timesinceHit = 0;
            }
            timesinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 KB)
    {
        if (IsAlive && !IsInvincible)
        {
            Health -= damage;
            IsInvincible = true;
            animator.SetTrigger(AnimationStrings.HitTrigger);
            damageableHit?.Invoke(damage, KB);

            return true;
        }
        else die();
        return false;
    }
    private void die()
    {
        if (gameObject.tag == "Player")
        {
            if (SceneManager.GetActiveScene().name == "man1" || SceneManager.GetActiveScene().name == "man2" || SceneManager.GetActiveScene().name == "man3")
            {
                SceneManager.LoadScene("Ending 2");
            }
            if (SceneManager.GetActiveScene().name == "Endless mode" || SceneManager.GetActiveScene().name == "BattleScene")
            {
                SceneManager.LoadScene("Cottruyen4");
            }
        }
    }
}
