using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUnlockManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public int cost;
        public bool isUnlocked;
        public Button unlockButton;
        public GameObject lockOverlay; // icon khoá
    }

    public CharacterData[] characters;
    public TextMeshProUGUI scoreText;

    private string currentUser;
    private int playerScore;

    void Start()
    {
        currentUser = PlayerPrefs.GetString("current_user", "");
        playerScore = PlayerPrefs.GetInt("score_" + currentUser, 0);

        LoadCharacters();
        UpdateUI();
    }

    void LoadCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            int unlocked = PlayerPrefs.GetInt("char_" + currentUser + "_" + i, 0);
            characters[i].isUnlocked = (unlocked == 1);

            // Cập nhật giao diện
            characters[i].lockOverlay.SetActive(!characters[i].isUnlocked);
            int index = i;
            characters[i].unlockButton.onClick.AddListener(() => UnlockCharacter(index));
        }
    }

    public void UnlockCharacter(int index)
    {
        if (characters[index].isUnlocked) return;

        if (playerScore >= characters[index].cost)
        {
            playerScore -= characters[index].cost;
            characters[index].isUnlocked = true;
            PlayerPrefs.SetInt("char_" + currentUser + "_" + index, 1);
            PlayerPrefs.SetInt("score_" + currentUser, playerScore);
            PlayerPrefs.Save();

            characters[index].lockOverlay.SetActive(false);
            UpdateUI();
        }
        else
        {
            Debug.Log("⚠ Không đủ điểm để mở khóa!");
        }
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + playerScore;
    }
}
