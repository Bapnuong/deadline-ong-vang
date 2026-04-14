using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class BossTest
{
    GameObject boss;
    DamageAble damage;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        boss = new GameObject("Boss");

        boss.AddComponent<Animator>();
        boss.AddComponent<Rigidbody2D>();
        boss.AddComponent<CapsuleCollider2D>();

        damage = boss.AddComponent<DamageAble>();

        yield return null; 
    }


    [UnityTest]
    public IEnumerator Player_Attack_Boss_Decrease_HP()
    {
        damage.Health = 100;

        yield return null;

        damage.Hit(10, Vector2.zero);

        yield return null;

        Assert.AreEqual(90, damage.Health);
    }


    [UnityTest]
    public IEnumerator Boss_Die_When_HP_Zero()
    {
        damage.Health = 10;

        yield return null;

        damage.Hit(10, Vector2.zero);

        yield return null;

        Assert.IsFalse(damage.IsAlive);
    }


    [UnityTest]
    public IEnumerator Boss_Die_And_Play_Death_Animation()
    {
        var animator = boss.GetComponent<Animator>();

        damage.Health = 10;

        yield return null;

        damage.Hit(10, Vector2.zero);

        yield return null;

        Assert.IsFalse(damage.IsAlive);

        Assert.IsNotNull(animator);
    }
}