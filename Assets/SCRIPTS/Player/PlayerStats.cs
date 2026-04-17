using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float baseDamage = 2f;
    public float attackSpeed = 1f;

    public float damageMultiplier = 1f;

    public float GetDamage()
    {
        return baseDamage * damageMultiplier;
    }
}