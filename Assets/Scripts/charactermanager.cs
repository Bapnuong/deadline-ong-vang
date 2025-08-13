using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Header("UI Shop")]
    public GameObject panelShop;
    public Button btnPrev;
    public Button btnNext;
    public Button btnSelect;
    public Image characterPreview;

    [Header("Characters")]
    public Sprite[] characterImages;           // ảnh dùng để hiển thị trong shop
    public GameObject[] characterPrefabs;      // prefab thật sẽ được spawn khi chọn
    public Transform spawnPoint;               // vị trí spawn nhân vật

    [Header("Boss Reference")]
    public BOSS1 boss;                         // Kéo thả boss vào đây
    public BOSS2 boss2;

    private int currentIndex = 0;
    private GameObject currentCharacterInGame;

    void Start()
    {
        btnPrev.onClick.AddListener(OnPrev);
        btnNext.onClick.AddListener(OnNext);
        btnSelect.onClick.AddListener(OnSelect);

        UpdateCharacterPreview();
    }

    void OnPrev()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = characterImages.Length - 1;
        UpdateCharacterPreview();
    }

    void OnNext()
    {
        currentIndex++;
        if (currentIndex >= characterImages.Length)
            currentIndex = 0;
        UpdateCharacterPreview();
    }

    void UpdateCharacterPreview()
    {
        if (characterImages.Length > 0)
            characterPreview.sprite = characterImages[currentIndex];
    }

    void OnSelect()
    {
        if (currentCharacterInGame != null)
            Destroy(currentCharacterInGame);

        currentCharacterInGame = Instantiate(characterPrefabs[currentIndex], spawnPoint.position, Quaternion.identity);


        panelShop.SetActive(false); // ẩn UI shop
    }
}
