using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class cottruyen : MonoBehaviour
{
    public Image storyImage;
    public Text storyText;

    [System.Serializable]
    public class Story
    {
        public Sprite image;
        [TextArea]
        public string text;
    }

    public Story[] stories;
    public float typingSpeed = 0.05f;
    public float delayBetweenStories = 1f;

    private void Start()
    {
        StartCoroutine(PlayStory());
    }

    IEnumerator PlayStory()
    {
        foreach (Story s in stories)
        {
            storyImage.sprite = s.image;
            storyText.text = "";

            // Chạy chữ
            foreach (char c in s.text)
            {
                storyText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Chờ rồi xóa
            yield return new WaitForSeconds(delayBetweenStories);
            storyImage.sprite = null;
            storyText.text = "";
        }
        if (SceneManager.GetActiveScene().name == "Ending 1" || SceneManager.GetActiveScene().name == "Ending 2")
        {
            SceneManager.LoadScene("UI");
        }
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
 