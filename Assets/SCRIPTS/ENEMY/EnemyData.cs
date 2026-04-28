using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float viewDistance = 10f;
    public int maxHealth = 10;

    [Header("Attack")]
    public AttackData attackData;   // all attack info lives here now

    [Header("References")]
    public GameObject modelPrefab;
    public RuntimeAnimatorController animator;
}