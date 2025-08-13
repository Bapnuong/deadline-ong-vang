using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class taikhoan : MonoBehaviour
{
    public TMP_InputField gmailInput;
    public TMP_InputField passwordInput;
    public Text messageText;
    public GameObject dangnhapform;

    private bool IsValidGmail(string gmail)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
        return Regex.IsMatch(gmail, pattern);
    }

    private bool GmailExists(string gmail)
    {
        return PlayerPrefs.HasKey("user_" + gmail);
    }

    private void Start()
    {
        // Nếu đã có người đăng nhập trước đó thì ẩn form
        if (PlayerPrefs.HasKey("current_user"))
        {
            dangnhapform.SetActive(false);
            messageText.text = "👋 Chào mừng trở lại, " + PlayerPrefs.GetString("current_user");
        }
    }

    public void Register()
    {
        string gmail = gmailInput.text.Trim();
        string password = passwordInput.text;

        if (!IsValidGmail(gmail))
        {
            messageText.text = "❌ Gmail không hợp lệ!";
            return;
        }

        if (GmailExists(gmail))
        {
            messageText.text = "❌ Gmail đã đăng ký!";
            return;
        }

        PlayerPrefs.SetString("user_" + gmail, password);
        PlayerPrefs.Save();

        // Lưu key để leaderboard dùng
        LeaderboardManager.AddKey("user_" + gmail);

        messageText.text = "✅ Đăng ký thành công!";
    }

    public void Login()
    {
        string gmail = gmailInput.text.Trim();
        string password = passwordInput.text;

        if (!GmailExists(gmail))
        {
            messageText.text = "❌ Gmail chưa đăng ký!";
            return;
        }

        string savedPassword = PlayerPrefs.GetString("user_" + gmail);
        if (savedPassword == password)
        {
            messageText.text = "✅ Đăng nhập thành công!";
            PlayerPrefs.SetString("current_user", gmail);
            PlayerPrefs.Save(); // Đảm bảo lưu lại
            dangnhapform.SetActive(false);
        }
        else
        {
            messageText.text = "❌ Mật khẩu sai!";
        }
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("current_user");
        PlayerPrefs.Save();
        dangnhapform.SetActive(true);
        messageText.text = "🚪 Đã đăng xuất!";
    }

    public void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        messageText.text = "🗑 Dữ liệu đã xóa.";
    }

    public void SetScore(string gmail, int score)
    {
        PlayerPrefs.SetInt("score_" + gmail, score);
        PlayerPrefs.Save();
    }

    public int GetScore(string gmail)
    {
        return PlayerPrefs.GetInt("score_" + gmail, 0);
    }

    public void AddScore(string gmail, int add)
    {
        int current = GetScore(gmail);
        SetScore(gmail, current + add);
    }
}
