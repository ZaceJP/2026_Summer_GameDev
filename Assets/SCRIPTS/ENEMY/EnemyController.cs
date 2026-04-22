using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public Transform player;

    private float lastAttackTime;
    private int currentHealth;

    private GameObject modelInstance;
    private CharacterController controller;

    private Room room;

    private void Start()
    {

        room = GetComponentInParent<Room>();
        controller = GetComponent<CharacterController>();

        currentHealth = data.maxHealth;

        if (data.modelPrefab == null ) 
        {
            modelInstance = Instantiate(data.modelPrefab, transform);
        }

        if (data.animator != null && modelInstance != null) 
        {
            Animator anim = modelInstance.GetComponent<Animator>();
            if (anim != null) 
            {
                anim.runtimeAnimatorController = data.animator;
            }
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }


    }
    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        //Check if player is in sight range
        if (distance <= data.viewDistance)
        {
            if (distance > data.attackRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                TryAttack();
            }

        }
        
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        direction = direction.normalized;

        // Stop if wall in front
        if (Physics.Raycast(transform.position, direction, 0.6f))
            return;

        Vector3 move = direction * data.moveSpeed;
        move.y = -1f; // keep grounded

        controller.Move(move * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + data.attackCooldown)
        {
            lastAttackTime = Time.time;

            PlayerHealth ph = player.GetComponent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(data.damage);
                Debug.Log("Enemy attacked player for " + data.damage + " damage!");
            }
        }
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (room != null)
        {
            room.OnEnemyDied();
        }

        Destroy(gameObject);
    }

   

}
