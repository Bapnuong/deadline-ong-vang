using UnityEngine;
using UnityEngine.Audio;

public class ambuton : MonoBehaviour
{
    public AudioClip buton;
    public AudioSource audioSource;

    public void buttonmusic()
    {
        if (audioSource != null && buton != null)
        {
            audioSource.PlayOneShot(buton);
        }
    }
}
