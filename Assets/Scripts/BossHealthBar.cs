using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private DamageAble currentBoss;

    public void SetBoss(DamageAble boss)
    {
        slider.maxValue = boss.MaxHealth;
        slider.value = boss.Health;

        boss.damageableHit.AddListener((damage, kb) =>
        {
            slider.value = boss.Health;
        });
    }


    private void UpdateHealthBar(float newHealth)
    {
        SetHealth(newHealth);
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
