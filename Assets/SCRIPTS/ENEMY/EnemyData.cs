using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float viewDistance = 10f;
    public float attackRange = 2f;
    public int damage = 1;
    public float attackCooldown = 1.5f;
    public int maxHealth = 10;

    [Header("References")]
    public GameObject modelPrefab;
    public RuntimeAnimatorController animator;

    [Header("Type of Enemy")]
    public bool isRanged;

    // [Header("Skills")]    
    // later we have a script for skills which we can connect

}