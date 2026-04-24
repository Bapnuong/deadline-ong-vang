using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class UITest_PlayMode
{
    // 🔥 Test toggle setting panel
    [UnityTest]
    public IEnumerator SettingPanel_Toggle_Works()
    {
        GameObject panel = new GameObject();
        panel.SetActive(false);

        UI ui = new GameObject().AddComponent<UI>();
        ui.settingpanel = panel;

        ui.setting();
        Assert.IsTrue(panel.activeSelf);

        ui.setting();
        Assert.IsFalse(panel.activeSelf);

        yield return null;
    }

    // 🔥 Test đăng nhập toggle
    [UnityTest]
    public IEnumerator DangNhap_Toggle_Works()
    {
        GameObject panel = new GameObject();
        panel.SetActive(false);

        UI ui = new GameObject().AddComponent<UI>();
        ui.dangnhapform = panel;

        ui.dangnhap();
        Assert.IsTrue(panel.activeSelf);

        ui.dangnhap();
        Assert.IsFalse(panel.activeSelf);

        yield return null;
    }

    // 🔥 Test vào game
    [UnityTest]
    public IEnumerator VaoGame_LoadScene()
    {
        UI ui = new GameObject().AddComponent<UI>();

        ui.vaogame();

        yield return null;

        Assert.AreEqual("Cottruyen1", SceneManager.GetActiveScene().name);
    }

    // 🔥 Test PvP
    [UnityTest]
    public IEnumerator PvP_LoadScene()
    {
        UI ui = new GameObject().AddComponent<UI>();

        ui.Pvp();

        yield return null;

        Assert.AreEqual("chonPvP", SceneManager.GetActiveScene().name);
    }

    // 🔥 Test Endless Boss
    [UnityTest]
    public IEnumerator EndlessBoss_LoadScene()
    {
        UI ui = new GameObject().AddComponent<UI>();

        ui.vohanboss();

        yield return null;

        Assert.AreEqual("Endless mode", SceneManager.GetActiveScene().name);
    }
}