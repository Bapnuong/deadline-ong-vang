using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public GameObject settingpanel;
    public GameObject rankingpanel;
    public GameObject dangkiform;
    public GameObject dangnhapform;
    public void dangki()
    {
        if(dangkiform.active == false)
        {
            dangkiform.SetActive(true);
        }
        else dangkiform.SetActive(false);
    }
    public void dangnhap()
    {
        if (dangnhapform.active == false)
        {
            dangnhapform.SetActive(true);
        }
        else dangnhapform.SetActive(false);
    }

    public void vaogame()
    {
        SceneManager.LoadScene("Cottruyen1");

    }

    public void setting()
    {
        if(settingpanel.active == false)
        {
            settingpanel.SetActive(true);
        }
        else settingpanel.SetActive(false);
    }

    public void thoatgame()
    {
        Application.Quit();
    }

    public void bangdiem()
    {
       if(rankingpanel.active == false)
        {
            rankingpanel.SetActive(true);
        }
       else rankingpanel.SetActive(false);
    }

    public void Pvp()
    {
        SceneManager.LoadScene("chonPvP");
    }

    public void EndlessMode()
    {
        SceneManager.LoadScene("Cottruyen4");
    }
    public void vohanboss()
    {
        SceneManager.LoadScene("Endless mode");

    }public void vegameui()
    {
        SceneManager.LoadScene("UI");

    }

}
