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
        var gamepad = InputSystem.GetDevice<Gamepad>();

        var player1 = PlayerInput.Instantiate(characterPrefabs[p1Index], controlScheme: "Keyboard&Mouse", pairWithDevice: keyboard);
        player1.transform.position = player1Spawn.position;
        player1.transform.rotation = Quaternion.identity;

        var player2 = PlayerInput.Instantiate(characterPrefabs[p2Index], controlScheme: "Gamepad", pairWithDevice: gamepad);
        player2.transform.position = player2Spawn.position;
        player2.transform.rotation = Quaternion.identity;
    }
}