using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private DamageAble currentBoss;
    [SerializeField] private BossHealthBar bossHealthBar;
    [SerializeField] private string[] bossPrefabNames; // Danh sách boss theo thứ tự

    private int currentBossIndex = 0; // Boss hiện tại

    void Start()
    {
        if (currentBoss != null)
        {
            RegisterBoss(currentBoss);
        }
        else
        {
            SpawnBoss(currentBossIndex); // Spawn boss đầu tiên nếu chưa có
        }
    }

    void RegisterBoss(DamageAble boss)
    {
        currentBoss = boss;
        bossHealthBar.SetBoss(currentBoss);
        currentBoss.onDeath.AddListener(OnBossDeath);
    }

    void OnBossDeath()
    {
        Debug.Log($"Boss {currentBossIndex + 1} chết");

        if (currentBoss != null)
        {
            currentBoss.onDeath.RemoveListener(OnBossDeath);
            Destroy(currentBoss.gameObject);
        }

        currentBossIndex++;
        if (currentBossIndex < bossPrefabNames.Length)
        {
            SpawnBoss(currentBossIndex);
        }
        else
        {
            Debug.Log("Không còn boss mới. Game thắng!");
            // Có thể gọi sự kiện kết thúc game ở đây
        }
    }

    void SpawnBoss(int index)
    {
        string prefabName = bossPrefabNames[index];
        GameObject bossPrefab = Resources.Load<GameObject>(prefabName);

        if (bossPrefab == null)
        {
            Debug.LogError($"Không tìm thấy {prefabName} trong Resources!");
            return;
        }

        GameObject bossObj = Instantiate(bossPrefab);
        DamageAble newBoss = bossObj.GetComponent<DamageAble>();

        if (newBoss != null)
        {
            RegisterBoss(newBoss);
        }
        else
        {
            Debug.LogError($"{prefabName} không có script DamageAble!");
        }
    }
}
