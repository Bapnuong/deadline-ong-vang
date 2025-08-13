using UnityEngine;
using UnityEngine.UI;

public class Trangthai : MonoBehaviour
{
    public Text statusText;

    public string[] statuses =
    {
        "Deadline dí quá rồi !!!!!!",
        "Buồn ngủ quá.....zzz",
        "Đang combat từ từ!!!",
        "Làm xong deadline rồi <3",
        "May quá nộp kịp Deadline -.-"
    };

    private void Start()
    {
        int randomIndex = Random.Range(0, statuses.Length); 
        statusText.text = statuses[randomIndex];
    }
}
