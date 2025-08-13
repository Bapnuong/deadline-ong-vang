using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip punchSFX;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPunchSFX()
    {
        audioSource.PlayOneShot(punchSFX);
    }
}
