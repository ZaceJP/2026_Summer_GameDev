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

    [Header("Transformation")]
    [SerializeField] private float transformDuration = 3f;
    [SerializeField] private GameObject transformVFX;

    private BossState currentState = BossState.Phase1;

    protected override void Start()
    {
      //  data = phase1Data;

        if (phase1Model != null)
            phase1Model.SetActive(true);

        if (phase2Model != null)
            phase2Model.SetActive(false);

        base.Start();
    }

    public override void TakeDamage(int amount)
    {
        if (currentState == BossState.Transforming ||
            currentState == BossState.Dead)
            return;

        base.TakeDamage(amount);

        if (currentState == BossState.Phase1)
        {
            float hpPercent =
                (float)CurrentHealth / data.maxHealth;

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

        data = phase2Data;

        currentHealth = data.maxHealth;

        currentState = BossState.Phase2;
    }

    protected override void Die()
    {
        GameEndManager.Instance.TriggerEndScreen(GameEndState.GameClear);

        base.Die();
    }
}