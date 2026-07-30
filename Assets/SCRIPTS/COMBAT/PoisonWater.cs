using UnityEngine;
using System.Collections;

public class PoisonWater : MonoBehaviour
{
    [Header("Poison Settings")]
    public int damagePerTick = 1;
    public float tickInterval = 1f;

    private Coroutine damageRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerStats player = other.GetComponent<PlayerStats>();

        if (player != null)
        {
            damageRoutine = StartCoroutine(DamageOverTime(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DamageOverTime(PlayerStats player)
    {
        while (true)
        {
            player.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }
}