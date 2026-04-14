using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
using System.Collections;
using System.Diagnostics;

public class GameplayIntegrationTests
{
    // ==================== MOCK CLASSES (tự chứa) ====================
    public class DamageAble : MonoBehaviour
    {
        public int Health = 100;
        public int MaxHealth = 100;
        public bool IsAlive => Health > 0;
        public UnityEvent<int, Vector2> damageableHit = new UnityEvent<int, Vector2>();

        public void Hit(int damage, Vector2 knockback)
        {
            Health = Mathf.Max(0, Health - damage);
            damageableHit?.Invoke(damage, knockback);
        }
    }

    public class PlayerControllerMock : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float moveSpeed = 5f;
        public float jumpForce = 7f;
        private bool grounded = true;

        private void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f;
        }

        public void Move(float direction)
        {
            var v = rb.linearVelocity;
            v.x = direction * moveSpeed;
            rb.linearVelocity = v;
        }

        public void Stop()
        {
            var v = rb.linearVelocity;
            v.x = 0f;
            rb.linearVelocity = v;
        }

        public void Jump()
        {
            if (!grounded) return;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }

        private void OnCollisionEnter2D(Collision2D other) => grounded = true;

        public void Attack(DamageAble target, int damage = 10, Vector2 kb = default)
        {
            target?.Hit(damage, kb);
        }
    }

    public class BossAIMock : MonoBehaviour
    {
        public Transform target;
        public float speed = 2f;
        public int baseDamage = 10;
        public bool IsPhase2 { get; private set; }
        public Animator animator;

        public int Damage => IsPhase2 ? baseDamage * 2 : baseDamage;

        private void Awake()
        {
            animator = gameObject.GetComponent<Animator>() ?? gameObject.AddComponent<Animator>();
        }

        private void Update()
        {
            if (target == null) return;
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            if (dir.x > 0) transform.localScale = Vector3.one;
            else if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        public void EvaluatePhase(int currentHealth, int maxHealth)
        {
            if (maxHealth <= 0) return;
            IsPhase2 = currentHealth < (maxHealth * 0.5f);
            if (animator != null) animator.SetBool("Phase2", IsPhase2);
        }

        public void AttackTarget(DamageAble target)
        {
            target?.Hit(Damage, Vector2.zero);
        }
    }

    public class HealthBarMock : MonoBehaviour
    {
        public int lastValue;
        public void Register(DamageAble d)
        {
            if (d == null) return;
            d.damageableHit.AddListener((damage, kb) => lastValue = d.Health);
        }
    }

    public class GameManagerMock : MonoBehaviour
    {
        public bool isGameOver;
        public Vector3 initialPlayerPos;
        public Transform player;

        public void RegisterPlayer(Transform t)
        {
            player = t;
            initialPlayerPos = t.position;
        }

        public void GameOver() => isGameOver = true;

        public void Restart()
        {
            isGameOver = false;
            if (player != null) player.position = initialPlayerPos;
        }
    }

    public class AudioManagerMock : MonoBehaviour
    {
        public int playCount;
        public void Play(string key) => playCount++;
    }

    // ==================== TESTS ====================
    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator Player_Move_Left_Right_Jump_Stop()
    {
        var playerGo = new GameObject("Player");
        var ctrl = playerGo.AddComponent<PlayerControllerMock>();

        ctrl.Move(1f);
        yield return null;
        Assert.Greater(ctrl.rb.linearVelocity.x, 0.1f);

        ctrl.Move(-1f);
        yield return null;
        Assert.Less(ctrl.rb.linearVelocity.x, -0.1f);

        ctrl.Stop();
        yield return null;
        Assert.AreEqual(0f, ctrl.rb.linearVelocity.x, 0.1f);

        ctrl.Jump();
        yield return new WaitForSeconds(0.02f);
        Assert.Greater(ctrl.rb.linearVelocity.y, 0f);

        Object.DestroyImmediate(playerGo);
    }

    [UnityTest]
    public IEnumerator Player_Attack_Boss_Decreases_HP_And_Knockback()
    {
        var playerGo = new GameObject("Player");
        var player = playerGo.AddComponent<PlayerControllerMock>();

        var bossGo = new GameObject("Boss");
        var damage = bossGo.AddComponent<DamageAble>();
        damage.Health = 100;

        player.Attack(damage, 15, new Vector2(2f, 0.5f));
        yield return null;

        Assert.AreEqual(85, damage.Health);

        var bossRb = bossGo.GetComponent<Rigidbody2D>() ?? bossGo.AddComponent<Rigidbody2D>();
        bossRb.AddForce(new Vector2(2f, 0.5f), ForceMode2D.Impulse);
        yield return null;
        Assert.AreNotEqual(Vector2.zero, bossRb.linearVelocity);

        Object.DestroyImmediate(playerGo);
        Object.DestroyImmediate(bossGo);
    }

    [UnityTest]
    public IEnumerator Boss_AI_Follow_Player_And_Phase2_Activates_When_HP_Low()
    {
        var playerGo = new GameObject("Player");
        var bossGo = new GameObject("Boss");
        var bossAI = bossGo.AddComponent<BossAIMock>();
        bossAI.target = playerGo.transform;
        bossAI.speed = 10f;

        yield return new WaitForSeconds(0.1f);
        Assert.Less(bossGo.transform.position.x, 5f);

        var damage = bossGo.AddComponent<DamageAble>();
        damage.Health = 40;
        damage.MaxHealth = 100;
        bossAI.EvaluatePhase(damage.Health, damage.MaxHealth);

        Assert.IsTrue(bossAI.IsPhase2);
        Assert.AreEqual(20, bossAI.Damage);

        Object.DestroyImmediate(playerGo);
        Object.DestroyImmediate(bossGo);
    }

    [UnityTest]
    public IEnumerator UI_HealthBar_Updates_On_Damage()
    {
        var bossGo = new GameObject("Boss");
        var damage = bossGo.AddComponent<DamageAble>();
        damage.Health = 100;

        var hbGo = new GameObject("HealthBar");
        var hb = hbGo.AddComponent<HealthBarMock>();
        hb.Register(damage);

        damage.Hit(25, Vector2.zero);
        yield return null;

        Assert.AreEqual(75, hb.lastValue);

        Object.DestroyImmediate(bossGo);
        Object.DestroyImmediate(hbGo);
    }

    [UnityTest]
    public IEnumerator GameOver_And_Restart_Resets_Player_Position()
    {
        var gmGo = new GameObject("GameManager");
        var gm = gmGo.AddComponent<GameManagerMock>();

        var playerGo = new GameObject("Player");
        playerGo.transform.position = new Vector3(1, 2, 3);
        gm.RegisterPlayer(playerGo.transform);

        gm.GameOver();
        playerGo.transform.position = Vector3.one * 999;
        yield return null;

        Assert.IsTrue(gm.isGameOver);

        gm.Restart();
        yield return null;

        Assert.IsFalse(gm.isGameOver);
        Assert.AreEqual(gm.initialPlayerPos, playerGo.transform.position);

        Object.DestroyImmediate(gmGo);
        Object.DestroyImmediate(playerGo);
    }

    [UnityTest]
    public IEnumerator Audio_Play_Is_Called_Once_Per_Event()
    {
        var audioGo = new GameObject("AudioManager");
        var am = audioGo.AddComponent<AudioManagerMock>();

        am.Play("attack");
        am.Play("attack");
        am.Play("death");
        yield return null;

        Assert.AreEqual(3, am.playCount);

        Object.DestroyImmediate(audioGo);
    }

    [UnityTest]
    public IEnumerator Performance_Simple_Stress_Test_No_Exceptions()
    {
        const int spawnCount = 200;
        var roots = new GameObject("PerfRoot");
        long memBefore = System.GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < spawnCount; i++)
        {
            var go = new GameObject("PerfObj_" + i);
            go.transform.parent = roots.transform;
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<PlayerControllerMock>();
        }

        for (int i = 0; i < 50; i++) yield return null;

        sw.Stop();
        long memDelta = System.GC.GetTotalMemory(false) - memBefore;

        Assert.Less(sw.ElapsedMilliseconds, 5000);
        Assert.Less(memDelta, 50 * 1024 * 1024);

        Object.DestroyImmediate(roots);
    }

    [UnityTest]
    public IEnumerator Integration_Attack_Leads_To_Boss_Death_And_Game_Progress()
    {
        var gmGo = new GameObject("GameManager");
        var gm = gmGo.AddComponent<GameManagerMock>();

        var playerGo = new GameObject("Player");
        var player = playerGo.AddComponent<PlayerControllerMock>();
        gm.RegisterPlayer(playerGo.transform);

        var bossGo = new GameObject("Boss");
        var damage = bossGo.AddComponent<DamageAble>();
        damage.Health = 30;
        damage.MaxHealth = 30;

        player.Attack(damage, 30, Vector2.zero);
        yield return null;

        Assert.IsFalse(damage.IsAlive);

        gm.GameOver();
        yield return null;
        Assert.IsTrue(gm.isGameOver);

        Object.DestroyImmediate(gmGo);
        Object.DestroyImmediate(playerGo);
        Object.DestroyImmediate(bossGo);
    }
}