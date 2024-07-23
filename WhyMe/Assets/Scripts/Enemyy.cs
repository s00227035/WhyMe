using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemyy : Character
{
    public AIPath aiPath; // Reference to the AIPath component
    public int damage = 40;
    public float attackRange = 3.8f;
    public float sprintMovementSpeed = 8.5f; // sprint speed
    public float slowedMovementSpeed = 2f; // slowed speed
    public float attackCooldown = 3f; // Cooldown between attacks
    public float slowedCooldown = 7f; // Time to remain slowed after attack

    private GameObject player;
    private float attackTimer; // Timer for attack cooldown
    private float slowedTimer; // Timer for slowed state

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>(); // Get the AIPath component

        base.Start();
        slowedTimer = 0f; // Start with no slowdown
        attackTimer = attackCooldown; // Initialize the attack timer
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < attackRange)
        {
            if (attackTimer <= 0f)
            {
                AttackPlayer();
                attackTimer = attackCooldown; // Reset the attack timer
                slowedTimer = slowedCooldown; // Start slowed timer after attack
            }
            else
            {
                attackTimer -= Time.deltaTime; // Count down the attack timer
            }
            SetState(CharacterState.Idle);
        }
        else
        {
            SetState(CharacterState.Run);

            // Determine movement speed based on slowed state
            if (slowedTimer > 0f)
            {
                aiPath.maxSpeed = slowedMovementSpeed; // Set the AIPath speed to slowed speed
                slowedTimer -= Time.deltaTime; // Decrement slowed timer
            }
            else if (distanceToPlayer > 5f)
            {
                aiPath.maxSpeed = movementSpeed;
            }
            else
            {
                aiPath.maxSpeed = sprintMovementSpeed; // Set the AIPath speed to sprint speed
            }
        }
        Debug.Log("distance to player: " + distanceToPlayer);
        SetAnimation();
    }

    private void SetAnimation()
    {
        animator.SetBool("IsMoving", aiPath.velocity.sqrMagnitude > 0.1f);
    }

    private void AttackPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        slowedTimer = slowedCooldown; // Reset the slowed timer after every attack
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
