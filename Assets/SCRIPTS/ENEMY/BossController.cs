using System.Collections;
using UnityEngine;

public enum BossState
{
    Phase1,
    Transforming,
    Phase2,
    Dead
}

public class BossController : EnemyController
{
    [Header("Phase Settings")]
    [SerializeField] private EnemyData phase1Data;
    [SerializeField] private EnemyData phase2Data;

    [Header("Models")]
    [SerializeField] private GameObject phase1Model;
    [SerializeField] private GameObject phase2Model;

    [Header("ATTACKS")]
    [SerializeField]
    private BossAttack[] phase1Attacks;

    [SerializeField]
    private BossAttack[] phase2Attacks;

    [Header("Transformation")]
    [SerializeField] private float transformDuration = 3f;
    [SerializeField] private GameObject transformVFX;

    [Header("Combat")]
    [SerializeField] private float attackInterval = 3f;

    private BossState currentState = BossState.Phase1;

    private float attackTimer;

    protected override void Start()
    {
        // Uncomment once Phase 1 and Phase 2 use different EnemyData
        // data = phase1Data;

        if (phase1Model != null)
            phase1Model.SetActive(true);

        if (phase2Model != null)
            phase2Model.SetActive(false);

        attackTimer = attackInterval;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        if (currentState == BossState.Transforming)
            return;

        if (player == null)
            return;

        // Don't do anything until the player is close enough
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > data.viewDistance)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            UseRandomAttack();
            attackTimer = attackInterval;
        }
    }

   public void UseRandomAttack()
    {
        BossAttack[] attacks =
            currentState == BossState.Phase1
            ? phase1Attacks
            : phase2Attacks;

        if (attacks.Length == 0)
            return;

        BossAttack attack =
            attacks[Random.Range(0, attacks.Length)];

        attack.Execute(this);
    }
    

    public override void TakeDamage(int amount)
    {
        if (currentState == BossState.Transforming ||
            currentState == BossState.Dead)
            return;

        base.TakeDamage(amount);

        if (currentState == BossState.Phase1)
        {
            float hpPercent = (float)CurrentHealth / data.maxHealth;

            if (hpPercent <= 0.5f)
            {
                StartCoroutine(TransformBoss());
            }
        }
    }

    IEnumerator TransformBoss()
    {
        currentState = BossState.Transforming;

        if (animator != null)
            animator.SetTrigger("Transform");

        if (transformVFX != null)
            Instantiate(transformVFX, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(transformDuration);

        if (phase1Model != null)
            phase1Model.SetActive(false);

        if (phase2Model != null)
            phase2Model.SetActive(true);

        if (phase2Data != null)
        {
            data = phase2Data;
            currentHealth = data.maxHealth;
        }

        currentState = BossState.Phase2;
    }

    protected override void Die()
    {
        currentState = BossState.Dead;

        GameEndManager.Instance.TriggerEndScreen(GameEndState.GameClear);

        base.Die();
    }

    public void PlayAnimation(BossAnimation animation)
    {
        if (animator == null)
            return;

        if (animation == BossAnimation.None)
            return;

        animator.SetTrigger(animation.ToString());
    }
}