using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;      // Panel pause
    public AudioSource gameAudio;       // Nhạc game
    public Button muteButton;           // Nút tắt tiếng
    public Button menuButton;           // Nút menu
    public Animator shopAnimator;       // Animator của nhân vật Shop (RawImage dùng camera phụ)

    private bool isMuted = false;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);   // Ẩn menu lúc đầu
        menuButton.onClick.AddListener(TogglePause);

        // Đảm bảo animator chạy ngay cả khi game pause
        if (shopAnimator != null)
        {
            shopAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;              // Dừng thời gian game
        isPaused = true;
        pauseMenuUI.SetActive(true);     // Hiện panel pause

        // Nếu animator chưa được set updateMode, thì set ở đây (phòng khi bị mất)
        if (shopAnimator != null)
        {
            shopAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenuUI.SetActive(false);
    }

    void ToggleMute()
    {
        isMuted = !isMuted;
        gameAudio.mute = isMuted;
    }
}
