using System.Collections;
using UnityEngine;

public class BossConeSmashAttack : BossAttack
{
    public float range = 6f;
    public float angle = 70f;

    public float warningTime = 1.5f;

    public int damage = 30;

    protected override void PerformAttack(BossController boss)
    {
        StartCoroutine(ConeRoutine(boss));
    }

    IEnumerator ConeRoutine(BossController boss)
    {
        Vector3 forward =
     (boss.player.position - boss.transform.position).normalized;

        forward.y = 0;

        yield return AttackSystem.Instance.Attack(
            IndicatorShape.Cone,
            boss.transform.position,
            Quaternion.LookRotation(forward),
            range,
            0f,
            angle,
            warningTime,
            damage);
    }
}