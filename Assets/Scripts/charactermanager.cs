using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CharacterSelector : MonoBehaviour
{
    [Header("UI Shop")]
    public GameObject panelShop;
    public Button btnPrev;
    public Button btnNext;
    public Button btnSelect;
    public TMP_Text scoreText;
    public TMP_Text unlockCostText;
    public Image characterPreview;
    public Image lockIcon; // Icon hình cái khóa

    [Header("Characters")]
    public Sprite[] characterImages;
    public GameObject[] characterPrefabs;
    public int[] unlockCosts;
    public Transform spawnPoint;

    private int currentIndex = 0;
    private GameObject currentCharacterInGame;

    private string currentUser;
    private int playerScore;
    private bool[] unlocked;

    void Start()
    {
        currentUser = PlayerPrefs.GetString("current_user", "");
        playerScore = PlayerPrefs.GetInt("score_" + currentUser, 0);

        // Load trạng thái mở khóa
        unlocked = new bool[characterImages.Length];
        for (int i = 0; i < unlocked.Length; i++)
        {
            unlocked[i] = PlayerPrefs.GetInt("char_" + currentUser + "_" + i, (i == 0 ? 1 : 0)) == 1;
        }

        btnPrev.onClick.AddListener(OnPrev);
        btnNext.onClick.AddListener(OnNext);
        btnSelect.onClick.AddListener(OnSelect);

        UpdateUI();
    }

    void OnPrev()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = characterImages.Length - 1;
        UpdateUI();
    }

    void OnNext()
    {
        currentIndex++;
        if (currentIndex >= characterImages.Length)
            currentIndex = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        characterPreview.sprite = characterImages[currentIndex];
        scoreText.text = "Score: " + playerScore;

        if (unlocked[currentIndex])
        {
            lockIcon.gameObject.SetActive(false);
            btnSelect.GetComponentInChildren<TMP_Text>().text = "Select";
            unlockCostText.text = "Unlocked";
        }
        else
        {
            lockIcon.gameObject.SetActive(true);
            Color c = lockIcon.color;
            c.a = 1f; // đảm bảo icon luôn hiện rõ khi bị khóa
            lockIcon.color = c;
            btnSelect.GetComponentInChildren<TMP_Text>().text = "Unlock";
            unlockCostText.text = "Cost: " + unlockCosts[currentIndex];
        }
    }

    void OnSelect()
    {
        if (!unlocked[currentIndex])
        {
            TryUnlock();
        }
        else
        {
            if (currentCharacterInGame != null)
                Destroy(currentCharacterInGame);

            currentCharacterInGame = Instantiate(characterPrefabs[currentIndex], spawnPoint.position, Quaternion.identity);
            panelShop.SetActive(false);
        }
    }

    void TryUnlock()
    {
        if (playerScore >= unlockCosts[currentIndex])
        {
            playerScore -= unlockCosts[currentIndex];
            unlocked[currentIndex] = true;
            PlayerPrefs.SetInt("char_" + currentUser + "_" + currentIndex, 1);
            PlayerPrefs.SetInt("score_" + currentUser, playerScore);
            PlayerPrefs.Save();

            StartCoroutine(FadeOutLockIcon());
        }
        else
        {
            Debug.Log("⚠ Không đủ điểm để mở khóa!");
        }
    }

    IEnumerator FadeOutLockIcon()
    {
        float duration = 0.5f;
        float t = 0f;
        Color c = lockIcon.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            lockIcon.color = c;
            yield return null;
        }

        lockIcon.gameObject.SetActive(false);
        UpdateUI();
    }
}
