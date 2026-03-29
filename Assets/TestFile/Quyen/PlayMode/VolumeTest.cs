using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VolumeTest
{
    [UnityTest]
    public IEnumerator AudioSourceVolume_SetToPointOneFive_PlayingHasCorrectVolume()
    {
        var go = new GameObject("TestAudio");
        var src = go.AddComponent<AudioSource>();

        // create a short silent clip to allow playing
        var clip = AudioClip.Create("test_clip", 44100, 1, 44100, false);
        src.clip = clip;

        src.volume = 0.15f; // just 15 pls -> 0.15 volume
        src.Play();

        // wait one frame so AudioSource updates
        yield return null;

        Assert.AreEqual(0.15f, src.volume, 1e-6f);
        Assert.IsTrue(src.isPlaying, "AudioSource should be playing after Play()");

        Object.DestroyImmediate(go);
    }
    [UnityTest]
    public IEnumerator AudioSourceVolume_SetToZero_PlayingHasZeroVolume()
    {
        var go = new GameObject("TestAudio");
        var src = go.AddComponent<AudioSource>();
        // create a short silent clip to allow playing
        var clip = AudioClip.Create("test_clip", 44100, 1, 44100, false);
        src.clip = clip;
        src.volume = 0f; // mute
        src.Play();
        // wait one frame so AudioSource updates
        yield return null;
        Assert.AreEqual(0f, src.volume, 1e-6f);
        Assert.IsTrue(src.isPlaying, "AudioSource should be playing after Play() even if volume is zero");
        Object.DestroyImmediate(go);
    }
    [UnityTest]

    public IEnumerator AudioSourceVolume_SetToOne_PlayingHasFullVolume()
    {
        var go = new GameObject("TestAudio");
        var src = go.AddComponent<AudioSource>();
        // create a short silent clip to allow playing
        var clip = AudioClip.Create("test_clip", 44100, 1, 44100, false);
        src.clip = clip;
        src.volume = 1f; // full volume
        src.Play();
        // wait one frame so AudioSource updates
        yield return null;
        Assert.AreEqual(1f, src.volume, 1e-6f);
        Assert.IsTrue(src.isPlaying, "AudioSource should be playing after Play() at full volume");
        Object.DestroyImmediate(go);

    }
    [UnityTest]
    public IEnumerator AudioSourceVolume_SetToNegative_ClampedToZero()
    {
        var go = new GameObject("TestAudio");
        var src = go.AddComponent<AudioSource>();
        // create a short silent clip to allow playing
        var clip = AudioClip.Create("test_clip", 44100, 1, 44100, false);
        src.clip = clip;
        src.volume = -0.5f; // invalid negative volume
        src.Play();
        // wait one frame so AudioSource updates
        yield return null;
        Assert.AreEqual(0f, src.volume, 1e-6f, "Volume should be clamped to zero when set to a negative value");
        Assert.IsTrue(src.isPlaying, "AudioSource should be playing after Play() even if volume is negative");
        Object.DestroyImmediate(go);
    }
    [UnityTest]
    public IEnumerator AudioSourceVolume_SetToAboveOne_ClampedToOne()
    {
        var go = new GameObject("TestAudio");
        var src = go.AddComponent<AudioSource>();
        // create a short silent clip to allow playing
        var clip = AudioClip.Create("test_clip", 44100, 1, 44100, false);
        src.clip = clip;
        src.volume = 1.5f; // invalid above one volume
        src.Play();
        // wait one frame so AudioSource updates
        yield return null;
        Assert.AreEqual(1f, src.volume, 1e-6f, "Volume should be clamped to one when set to a value above one");
        Assert.IsTrue(src.isPlaying, "AudioSource should be playing after Play() even if volume is above one");
        Object.DestroyImmediate(go);
    }
}