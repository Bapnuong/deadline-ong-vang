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

        // Gán layer Player2
        SetLayerRecursively(player2.gameObject, LayerMask.NameToLayer("Player2"));
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
