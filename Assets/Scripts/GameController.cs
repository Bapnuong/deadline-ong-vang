using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI timeText;

    public float gameDuration = 60f;
    private float currentTime;

    private int score = 0;
    private int combo = 0;
    private bool isGameOver = false;

    void Start()
    {
        currentTime = gameDuration;
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        timeText.text = Mathf.CeilToInt(currentTime) + "s";

        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    public void OnPlayerHitTarget(bool isPerfect = false)
    {
        combo++;

        float multiplier = 1f + (combo / 10f);
        int baseScore = 10;
        int comboScore = Mathf.RoundToInt(baseScore * multiplier);

        if (isPerfect) comboScore += 50;

        score += comboScore;
        UpdateUI();
    }

    public void OnPlayerMiss()
    {
        combo = 0;
        score -= 5;
        if (score < 0) score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        comboText.text = "Hit: " + combo;
    }

    void EndGame()
    {
        isGameOver = true;

        int timeBonus = Mathf.CeilToInt(currentTime) * 5;
        score += timeBonus;
        UpdateUI();

        Debug.Log("🛑 Game Over! Final Score: " + score);

        // Lưu điểm trực tiếp vào PlayerPrefs
        string currentUser = PlayerPrefs.GetString("current_user", "");
        if (!string.IsNullOrEmpty(currentUser))
        {
            int currentScore = PlayerPrefs.GetInt("score_" + currentUser, 0);
            PlayerPrefs.SetInt("score_" + currentUser, currentScore + score);
            PlayerPrefs.Save();

            Debug.Log("💾 Điểm đã lưu cho user: " + currentUser);
        }
    }
}
