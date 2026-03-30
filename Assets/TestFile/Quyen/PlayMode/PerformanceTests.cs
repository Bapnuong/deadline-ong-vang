using System.Collections;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class PerformanceTests
{
    private const string MENU_SCENE = "UI";

    [UnityTest]
    public IEnumerator TC15_SceneLoadTime_UI()
    {
        var sw = Stopwatch.StartNew();
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        sw.Stop();

        Assert.Less(sw.ElapsedMilliseconds, 1500, $"Load scene UI quá lâu: {sw.ElapsedMilliseconds}ms");
        Assert.IsNotNull(AudioManager.Instance);
    }

    // TC27
    [UnityTest]
    public IEnumerator TC16_FPS_Menu_30s()
    {
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1f);

        float totalTime = 0f;
        int frames = 0;
        float minFPS = float.MaxValue;

        for (int i = 0; i < 1800; i++) // 30 giây
        {
            totalTime += Time.deltaTime;
            frames++;
            float fps = 1f / Time.deltaTime;
            if (fps < minFPS) minFPS = fps;
            yield return null;
        }

        float avgFPS = frames / totalTime;
        Assert.GreaterOrEqual(avgFPS, 35f, $"FPS trung bình Editor chỉ {avgFPS:F1}");
        Assert.GreaterOrEqual(minFPS, 20f, $"FPS thấp nhất chỉ {minFPS:F1}");
    }

    // TC28
    [UnityTest]
    public IEnumerator TC17_MemoryUsage_OnLoadMenu()
    {
        long memoryBefore = Profiler.GetTotalAllocatedMemoryLong();
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1f);

        long memoryAfter = Profiler.GetTotalAllocatedMemoryLong();
        long increase = memoryAfter - memoryBefore;

        Assert.Less(increase / (1024f * 1024f), 30f, $"Bộ nhớ tăng quá nhiều: {increase / (1024f * 1024f):F1} MB");
    }

    // TC29
    [UnityTest]
    public IEnumerator TC18_GCSpike_WhenChangeMusic()
    {
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1f);

        long gcBefore = Profiler.GetMonoUsedSizeLong();
        // Simulate chuyển scene để trigger ChangeMusicSmoothly
        yield return SceneManager.LoadSceneAsync("man1", LoadSceneMode.Single);
        yield return new WaitForSeconds(2f);

        long gcAfter = Profiler.GetMonoUsedSizeLong();
        long spike = gcAfter - gcBefore;

        Assert.Less(spike / (1024f * 1024f), 5f, $"GC spike quá lớn: {spike / (1024f * 1024f):F1} MB");
    }

    // TC30 - Đã fix theo Inspector của bạn (nút tên "Start")
    [UnityTest]
    public IEnumerator TC19_UI_ButtonStressTest()
    {
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1.5f);

        GameObject startObj = GameObject.Find("Start");
        Assert.IsNotNull(startObj, "Không tìm thấy GameObject tên 'Start' trong Hierarchy!");

        Button startButton = startObj.GetComponent<Button>();
        Assert.IsNotNull(startButton, "GameObject 'Start' không có component Button!");

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            startButton.onClick.Invoke();
            yield return null;
        }
        sw.Stop();

        Assert.Less(sw.ElapsedMilliseconds, 800, $"100 lần click lag quá lâu: {sw.ElapsedMilliseconds}ms");
    }

    // TC31
    [UnityTest]
    public IEnumerator TC20_SceneTransitionTime_MenuToMan1()
    {
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1f);

        var sw = Stopwatch.StartNew();
        yield return SceneManager.LoadSceneAsync("man1", LoadSceneMode.Single);
        sw.Stop();

        Assert.Less(sw.ElapsedMilliseconds, 2000, $"Chuyển scene Menu → man1 quá lâu: {sw.ElapsedMilliseconds}ms");
    }

    // TC32 - Long run 3 phút
    [UnityTest]
    public IEnumerator TC21_LongRun_Menu_3Minutes()
    {
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1f);

        float totalTime = 0f;
        int frames = 0;
        long memoryStart = Profiler.GetTotalAllocatedMemoryLong();

        for (int i = 0; i < 10800; i++) // ~3 phút
        {
            totalTime += Time.deltaTime;
            frames++;
            yield return null;
        }

        float avgFPS = frames / totalTime;
        long memoryEnd = Profiler.GetTotalAllocatedMemoryLong();
        float memoryIncreaseMB = (memoryEnd - memoryStart) / (1024f * 1024f);

        Assert.GreaterOrEqual(avgFPS, 35f, $"FPS trung bình sau 3 phút chỉ {avgFPS:F1}");
        Assert.Less(memoryIncreaseMB, 15f, $"Memory leak: tăng {memoryIncreaseMB:F1} MB");
    }

    // TC33
    [UnityTest]
    public IEnumerator TC22_AudioManager_InitTime()
    {
        var sw = Stopwatch.StartNew();
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        sw.Stop();

        Assert.Less(sw.ElapsedMilliseconds, 300, $"Khởi tạo AudioManager quá lâu: {sw.ElapsedMilliseconds}ms");
    }

    // TC
    [UnityTest]
    public IEnumerator TC23_MultipleSceneSwitch_5Times()
    {
        var sw = Stopwatch.StartNew();
        string[] scenes = { "UI", "man1", "Cottruyen1", "man1", "UI" };

        for (int i = 0; i < scenes.Length; i++)
        {
            yield return SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Single);
            yield return new WaitForSeconds(0.8f);
        }
        sw.Stop();

        Assert.Less(sw.ElapsedMilliseconds, 10000, $"Chuyển 5 scene quá lâu: {sw.ElapsedMilliseconds}ms");
    }

    // TC35
    [UnityTest]
    public IEnumerator TC24_FrameTime_Consistency_60s()
    {
        yield return SceneManager.LoadSceneAsync(MENU_SCENE);
        yield return new WaitForSeconds(1f);

        int badFrames = 0;
        for (int i = 0; i < 3600; i++) // 60 giây
        {
            if (Time.deltaTime > 0.05f) badFrames++; // > 50ms = stutter
            yield return null;
        }

        float badPercent = (badFrames / 3600f) * 100f;
        Assert.Less(badPercent, 5f, $"Có quá nhiều frame stutter: {badPercent:F1}%");
    }
}