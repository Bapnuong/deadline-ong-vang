using UnityEngine;

public class attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 KB = Vector2.zero;

    private GameController gameController;

    private void Start()
    {
        // Tự động tìm GameController trong scene
        gameController = FindObjectOfType<GameController>();

        if (gameController == null)
        {
            Debug.LogWarning("⚠ Không tìm thấy GameController trong scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageAble damageAble = collision.GetComponent<DamageAble>();

        if (damageAble != null)
        {
            float direction = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 directionalKB = new Vector2(KB.x * direction, KB.y);

            bool gotHit = damageAble.Hit(attackDamage, directionalKB);

            if (gotHit)
            {
                Debug.Log(collision.name + " hit for " + attackDamage);

                // Báo cho GameController biết là trúng mục tiêu
                if (gameController != null)
                {
                    gameController.OnPlayerHitTarget();
                }
            }
        }
        else
        {
            // Nếu va chạm nhưng không phải mục tiêu thì coi như miss
            if (gameController != null)
            {
                gameController.OnPlayerMiss();
            }
        }
    }
}
