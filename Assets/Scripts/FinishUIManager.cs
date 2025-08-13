using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class FinishUIManager : MonoBehaviour
{
    public GameObject finishedPanel;
    public TextMeshProUGUI finishedText;
    public float scaleDuration = 0.6f;

    public void ShowFinished()
    {
        finishedPanel.SetActive(true);
        Debug.Log("SHOW FINISHED CALLED");
        StartCoroutine(ScaleText(finishedText.rectTransform));
    }

    IEnumerator ScaleText(Transform textTransform)
    {
        float timer = 0f;
        textTransform.localScale = Vector3.zero;

        while (timer < scaleDuration)
        {
            float t = timer / scaleDuration;
            textTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            timer += Time.deltaTime;
            yield return null;
        }

        textTransform.localScale = Vector3.one;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
