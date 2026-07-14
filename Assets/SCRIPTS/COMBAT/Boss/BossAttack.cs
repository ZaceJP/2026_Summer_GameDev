using UnityEngine;

public enum BossAnimation
{
    None,
    CircleSmash,
    ConeSmash,
    Transform,
    Hit,
    Death
}

public abstract class BossAttack : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string attackName = "Attack";

    [SerializeField] private float cooldown = 5f;

    [Header("Animation")]
    [SerializeField] private BossAnimation animation = BossAnimation.None;

    protected float lastUseTime;

    public bool IsReady()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void Execute(BossController boss)
    {
        if (!IsReady())
            return;

        lastUseTime = Time.time;

        boss.PlayAnimation(animation);

        PerformAttack(boss);
    }

    protected abstract void PerformAttack(BossController boss);
}