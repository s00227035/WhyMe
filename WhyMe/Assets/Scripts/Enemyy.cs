using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemyy : Character
{
    public AIPath aiPath; // Reference to the AIPath component
    public int damage = 30;
    public float attackRange = 5f;
    public float sprintMovementSpeed = 10f; // sprint speed
    public float slowedMovementSpeed = 2f; // slowed speed
    public float attackCooldown = 2f; // Cooldown between attacks
    public float slowedCooldown = 7f; // Time to remain slowed after attack
    
    private GameObject player;
    private float attackTimer; // Timer for attack cooldown
    private float slowedTimer; // Timer for slowed state
    //Audio
    private AudioSource runAudioSource; //Walk sound
    private AudioSource attackAudioSource; //Attack soind
    public AudioClip attackSound;
    public AudioClip runSound;
    public float soundTriggerRange = 20f; //Range for the enemy run sound

    //UI Dialogue pop up text
    private EnemyDialogue enemyDialogue;
    private string[] attackMessages = { "Got you!", "I'll kill you", "You'll die!" };

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>(); // Get the AIPath component

        base.Start();
        slowedTimer = 0f; // Start with no slowdown
        attackTimer = attackCooldown; // Initialize the attack timer

        //Initialize AudioSOurce componenet
        AudioSource[] audioSources = GetComponents<AudioSource>(); 
        runAudioSource = audioSources[0];
        attackAudioSource = audioSources[1];

        //Get Dialogue component
        enemyDialogue = GetComponentInChildren<EnemyDialogue>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Detect nearby box
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, attackRange, LayerMask.GetMask("Box"));
        if (hit.collider != null)
        {
            //If box detected in path, push it
            BoxInteraction(hit.collider.gameObject);
        }
        else
        {
            //Usual chase and attack
            MovementAndAttack(distanceToPlayer);
        }
        
        SetAnimation();
    }

    private void MovementAndAttack(float distanceToPlayer)
    {

    
        if (distanceToPlayer < attackRange)
        {
            if (attackTimer <= 0f)
            {
                //UI dialogue enemy
                if (enemyDialogue != null)
                {
                    string randomMessage = attackMessages[Random.Range(0, attackMessages.Length)];
                    enemyDialogue.ShowDialogue(randomMessage);
                }

                AttackPlayer();
                attackTimer = attackCooldown; // Reset the attack timer
                slowedTimer = slowedCooldown; // Start slowed timer after attack

                //Apply the slowed speed
                aiPath.maxSpeed = slowedMovementSpeed;
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
            else if (distanceToPlayer > 7f)
            {
                aiPath.maxSpeed = movementSpeed;
            }
            else
            {
                aiPath.maxSpeed = sprintMovementSpeed; // Set the AIPath speed to sprint speed
            }
        }
        //Debug.Log("distance to player: " + distanceToPlayer);

        if (distanceToPlayer <= soundTriggerRange)
        {
            PlayRunSound(); //Walk sound when moving
        }
        else
        {
            runAudioSource.Stop(); //Stop if the enemy is far
        }

    }
    private void BoxInteraction(GameObject box)
    {
        DraggableBox draggableBox = box.GetComponent<DraggableBox>();
        if (draggableBox != null)
        {
            //Push the box
            Vector2 pushDirection = (box.transform.position - transform.position).normalized;
            box.transform.position += (Vector3)(pushDirection * 0.4f); //Push amount
            
        }
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
            PlayAttackSound(); //Attak sound when attacking
        }
        slowedTimer = slowedCooldown; // Reset the slowed timer after every attack
    }

    private void PlayAttackSound()
    {
        if (attackSound != null && attackAudioSource != null)
        {
            attackAudioSource.PlayOneShot(attackSound);
        }
    }

    private void PlayRunSound()
    {
        if (runSound != null && runAudioSource != null && aiPath.velocity.sqrMagnitude > 0.1f)
        {
            if (!runAudioSource.isPlaying) //check is run sound is alrdy playing
            {
                runAudioSource.clip = runSound;
                runAudioSource.loop = true; //Looping
                runAudioSource.Play(); //Start plaing run sound
            }
        }
        else if (aiPath.velocity.sqrMagnitude <= 0.1f && runAudioSource.isPlaying)
        {
            runAudioSource.Stop(); //Stop the run sound when the enemy stops moving
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
