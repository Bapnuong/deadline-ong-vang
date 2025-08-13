using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider; // Thanh máu UI
    public DamageAble damageable; // Script gốc của nhân vật

    void Awake()
    {
        if (healthSlider == null)
        {
            GameObject sliderObj = GameObject.Find("heath"); // Tên đúng trong Hierarchy
            if (sliderObj != null)
            {
                healthSlider = sliderObj.GetComponent<Slider>();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject tên 'HealthSlider' trong scene.");
            }
        }
    }


    void Start()
    {
        if (damageable != null && healthSlider != null)
        {
            healthSlider.maxValue = damageable.MaxHealth;
            healthSlider.value = damageable.Health;
        }
    }

    void Update()
    {
        if (damageable != null && healthSlider != null)
        {
            healthSlider.value = damageable.Health;
        }
    }
}
