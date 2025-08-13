using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public taikhoan accountManager;

    void Start()
    {
        ShowTop5();
    }

    public void ShowTop5()
    {
        List<UserScore> scores = new List<UserScore>();

        foreach (var key in PlayerPrefsKeys())
        {
            if (key.StartsWith("user_"))
            {
                string gmail = key.Substring(5);
                int score = accountManager.GetScore(gmail);
                scores.Add(new UserScore(gmail, score));
            }
        }

        var top5 = scores.OrderByDescending(s => s.score).Take(5).ToList();

        leaderboardText.text = "🏆 Leaderboard 🏆\n";
        if (top5.Count == 0)
        {
            leaderboardText.text += "Chưa có ai trong bảng xếp hạng!";
            return;
        }

        for (int i = 0; i < top5.Count; i++)
        {
            leaderboardText.text += $"{i + 1}. {top5[i].gmail} - {top5[i].score} pts\n";
        }
    }

    private IEnumerable<string> PlayerPrefsKeys()
    {
        string allKeys = PlayerPrefs.GetString("all_keys", "");
        return allKeys.Split('|').Where(k => !string.IsNullOrEmpty(k));
    }

    public static void AddKey(string key)
    {
        string allKeys = PlayerPrefs.GetString("all_keys", "");
        if (!allKeys.Contains(key))
        {
            allKeys += key + "|";
            PlayerPrefs.SetString("all_keys", allKeys);
        }
    }
}

public struct UserScore
{
    public string gmail;
    public int score;

    public UserScore(string gmail, int score)
    {
        this.gmail = gmail;
        this.score = score;
    }
}
