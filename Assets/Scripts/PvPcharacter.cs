using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Thêm dòng này để dùng TextMeshPro

public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Sprite[] previewSprites; // Mảng ảnh đại diện
    public Image previewImage;
    public TextMeshProUGUI instructionText; // Đã sửa kiểu dữ liệu

    private int currentIndex = 0;
    private int player1Choice = -1;
    private int player2Choice = -1;
    private int currentPlayer = 1;

    void Start()
    {

        UpdatePreview();
        instructionText.text = "Player 1";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PreviousCharacter();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextCharacter();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            ConfirmSelection();
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characterPrefabs.Length;
        UpdatePreview();

        // Gọi âm thanh chuyển ảnh
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.chuyenhinh();
        }
    }

    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characterPrefabs.Length) % characterPrefabs.Length;
        UpdatePreview();

        // Gọi âm thanh chuyển ảnh
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.chuyenhinh();
        }
    }


    public void ConfirmSelection()
    {
        if (currentPlayer == 1)
        {
            player1Choice = currentIndex;
            currentPlayer = 2;
            currentIndex = 0;
            UpdatePreview();
            instructionText.text = "Player 2";
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.muanhanvat();
            }
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.muanhanvat();
            }
            player2Choice = currentIndex;
            instructionText.text = "Loading...";
            LoadBattleScene();
        }
    }

    void UpdatePreview()
    {
        if (previewSprites.Length > currentIndex && previewSprites[currentIndex] != null)
        {
            previewImage.sprite = previewSprites[currentIndex];
        }
        else
        {
            Debug.LogWarning("Không tìm thấy sprite preview cho index: " + currentIndex);
            previewImage.sprite = null;
        }
    }
    public void loadhome()
    {
        SceneManager.LoadScene("UI");
    }

    void LoadBattleScene()
    {
        PlayerPrefs.SetInt("Player1Character", player1Choice);
        PlayerPrefs.SetInt("Player2Character", player2Choice);
        SceneManager.LoadScene("BattleScene");
    }
}