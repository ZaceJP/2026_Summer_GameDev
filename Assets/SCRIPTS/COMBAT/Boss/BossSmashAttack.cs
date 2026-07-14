using System.Collections;
using UnityEngine;

public class BossSmashAttack : BossAttack
{
    public float radius = 3f;
    public float warningTime = 2f;
    public int damage = 40;

    protected override void PerformAttack(BossController boss)
    {
        StartCoroutine(SmashRoutine(boss));
    }

    IEnumerator SmashRoutine(BossController boss)
    {
        yield return AttackSystem.Instance.Attack(
    IndicatorShape.Circle,
    boss.player.position,
    Quaternion.identity,
    radius,
    0f,
    0f,
    warningTime,
    damage);
    }
}