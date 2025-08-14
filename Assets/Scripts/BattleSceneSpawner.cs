using UnityEngine;
using UnityEngine.InputSystem;

public class BattleSceneSpawner : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;
    public GameObject[] characterPrefabs;

    void Start()
    {
        int p1Index = PlayerPrefs.GetInt("Player1Character", 0);
        int p2Index = PlayerPrefs.GetInt("Player2Character", 0);

        var keyboard = InputSystem.GetDevice<Keyboard>();

        // Spawn Player 1 - Keyboard1
        var player1 = PlayerInput.Instantiate(
            characterPrefabs[p1Index],
            controlScheme: "Keyboard1",
            pairWithDevice: keyboard
        );
        player1.transform.position = player1Spawn.position;
        player1.transform.rotation = Quaternion.identity;

        // Spawn Player 2 - Keyboard2
        var player2 = PlayerInput.Instantiate(
            characterPrefabs[p2Index],
            controlScheme: "Keyboard2",
            pairWithDevice: keyboard
        );
        player2.transform.position = player2Spawn.position;
        player2.transform.rotation = Quaternion.identity;

        // Gán layer cho Player 2
        SetPlayer2Layers(player2.gameObject);
    }

    void SetPlayer2Layers(GameObject player)
    {
        // Đổi layer nhân vật thành Player2
        SetLayerSafe(player, "Player2");

        foreach (Transform child in player.GetComponentsInChildren<Transform>(true))
        {
            // Nếu là hitbox tấn công → gán Player2Hitbox
            if (child.name.ToLower().Contains("attack"))
            {
                SetLayerSafe(child.gameObject, "Player2Hitbox");
            }
            else
            {
                // Các phần khác → gán Player2
                SetLayerSafe(child.gameObject, "Player2");
            }
        }
    }

    /// <summary>
    /// Gán layer an toàn, báo lỗi nếu layer không tồn tại
    /// </summary>
    private void SetLayerSafe(GameObject obj, string layerName)
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogError($"[BattleSceneSpawner] Layer '{layerName}' không tồn tại. Kiểm tra Project Settings > Tags and Layers.");
            return;
        }
        obj.layer = layerIndex;
    }
}
