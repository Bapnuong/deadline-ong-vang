using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider; // Thanh máu UI
    public DamageAble damageable; // Script gốc của nhân vật


    void Start()
    {
        if (healthSlider == null)
        {
            GameObject sliderObj = null;
            string layerName = LayerMask.LayerToName(transform.root.gameObject.layer);
            Debug.Log($"{gameObject.name} đang ở layer: {layerName}");

            if (layerName == "Player1")
            {
                sliderObj = GameObject.Find("heath");
            }
            else if (layerName == "Player2")
            {
                sliderObj = GameObject.Find("heath boss");
            }
            else
            {
                sliderObj = GameObject.Find("heath");
            }

            if (sliderObj != null)
            {
                healthSlider = sliderObj.GetComponent<Slider>();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject thanh máu.");
            }
        }
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
