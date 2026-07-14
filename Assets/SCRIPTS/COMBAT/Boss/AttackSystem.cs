using UnityEngine;
using System.Collections;

public class AttackSystem : MonoBehaviour
{
    public static AttackSystem Instance;

    [SerializeField]
    private AttackIndicator attackIndicatorPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator Attack(
        IndicatorShape shape,
        Vector3 position,
        Quaternion rotation,
        float width,
        float length,
        float angle,
        float warningTime,
        int damage)
    {
        AttackIndicator indicator =
            Instantiate(
                attackIndicatorPrefab,
                position,
                rotation);

        indicator.Initialize(
            shape,
            width,
            length,
            angle);

        yield return indicator.Play(warningTime);

        Destroy(indicator.gameObject);

        switch (shape)
        {
            case IndicatorShape.Circle:
                DoCircleDamage(position, width, damage);
                break;

            case IndicatorShape.Cone:
                DoConeDamage(
                    position,
                    rotation * Vector3.forward,
                    angle,
                    width,
                    damage);
                break;

            case IndicatorShape.Rectangle:
                DoRectangleDamage(
                    position,
                    rotation,
                    width,
                    length,
                    damage);
                break;

            case IndicatorShape.Ring:
                DoRingDamage(
                    position,
                    width,
                    damage);
                break;
        }
    }

    // ----------------------------------------------------
    // Circle
    // ----------------------------------------------------

    void DoCircleDamage(
        Vector3 position,
        float radius,
        int damage)
    {
        Collider[] hits =
            Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hits)
        {
            PlayerStats player =
                hit.GetComponent<PlayerStats>();

            if (player != null)
                player.TakeDamage(damage);
        }
    }

    // ----------------------------------------------------
    // Cone
    // ----------------------------------------------------

    void DoConeDamage(
        Vector3 origin,
        Vector3 forward,
        float angle,
        float range,
        int damage)
    {
        Collider[] hits =
            Physics.OverlapSphere(origin, range);

        foreach (Collider hit in hits)
        {
            PlayerStats player =
                hit.GetComponent<PlayerStats>();

            if (player == null)
                continue;

            Vector3 dir =
                (player.transform.position - origin).normalized;

            if (Vector3.Angle(forward, dir) <= angle * 0.5f)
            {
                player.TakeDamage(damage);
            }
        }
    }

    // ----------------------------------------------------
    // Rectangle
    // ----------------------------------------------------

    void DoRectangleDamage(
        Vector3 position,
        Quaternion rotation,
        float width,
        float length,
        int damage)
    {
        Vector3 halfExtents =
            new Vector3(
                width * 0.5f,
                1f,
                length * 0.5f);

        Collider[] hits =
            Physics.OverlapBox(
                position,
                halfExtents,
                rotation);

        foreach (Collider hit in hits)
        {
            PlayerStats player =
                hit.GetComponent<PlayerStats>();

            if (player != null)
                player.TakeDamage(damage);
        }
    }

    // ----------------------------------------------------
    // Ring
    // ----------------------------------------------------

    void DoRingDamage(
        Vector3 position,
        float radius,
        int damage)
    {
        Collider[] hits =
            Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hits)
        {
            PlayerStats player =
                hit.GetComponent<PlayerStats>();

            if (player == null)
                continue;

            float distance =
                Vector3.Distance(
                    position,
                    player.transform.position);

            // Example: inner 50% is safe
            if (distance >= radius * 0.5f)
            {
                player.TakeDamage(damage);
            }
        }
    }
}