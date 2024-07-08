using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    public int Damage = 40;
    public float MinMovementSpeed = 1f;
    public float MaxMovementSpeed = 3f;
    public float AttackRange = 3f;
    public float FollowRange = 20f;
    public float searchDuration = 5f;
    public float wanderRadius = 10f;
    public float wanderTimer = 10f;
    public float attackCooldown = 2f; // Cooldown between attacks

    private GameObject player;
    private Vector3 lastKnownPlayerPosition;
    private bool isSearching;
    private float searchTimer;
    private float wanderTimerCurrent;
    private float attackTimer; // Timer for attack cooldown
    private Vector3 currentDirection;

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementSpeed = Random.Range(MinMovementSpeed, MaxMovementSpeed);
        animator = GetComponent<Animator>();

        base.Start();

        searchTimer = searchDuration;
        wanderTimerCurrent = wanderTimer;
        attackTimer = attackCooldown; // Initialize the attack timer
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Distance to player: " + distanceToPlayer);

        if (distanceToPlayer < AttackRange)
        {
            if (attackTimer <= 0f)
            {
                AttackPlayer();
                attackTimer = attackCooldown; // Reset the attack timer
                Debug.Log("Attacking player, resetting attack timer");
            }
            else
            {
                attackTimer -= Time.deltaTime; // Count down the attack timer
                Debug.Log("Attack timer: " + attackTimer);
            }
            SetState(CharacterState.Idle);
        }
        else if (distanceToPlayer < FollowRange)
        {
            bool canSeePlayer = CanSeePlayer();
            Debug.Log("Can see player: " + canSeePlayer);

            if (canSeePlayer)
            {
                SetState(CharacterState.Run);
                lastKnownPlayerPosition = player.transform.position;
                isSearching = false;
                UpdateSpriteDirection(player.transform.position - transform.position);
                currentDirection = (player.transform.position - transform.position).normalized;
                Debug.Log("Player within follow range and visible, switching to Run");
            }
            else
            {
                StartSearching();
                Debug.Log("Player within follow range but not visible, starting search mode");
            }
        }
        else if (isSearching)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0)
            {
                SetState(CharacterState.Idle);
                StartWandering();
                Debug.Log("Search timer expired, switching to Idle");
            }
            else
            {
                SetState(CharacterState.Run);
                UpdateSpriteDirection(lastKnownPlayerPosition - transform.position);
                currentDirection = (lastKnownPlayerPosition - transform.position).normalized;
                Debug.Log("Searching for player, moving to last known position");
            }
        }
        else
        {
            SetState(CharacterState.Idle);
            Debug.Log("Player not in follow range, switching to Idle");
        }

        SetAnimation();
    }

    private void FixedUpdate()
    {
        if (State == CharacterState.Run)
        {
            Vector3 direction;
            if (isSearching)
            {
                direction = (lastKnownPlayerPosition - transform.position).normalized;
            }
            else
            {
                direction = (player.transform.position - transform.position).normalized;
            }

            if (CanMoveInDirection(direction))
            {
                currentDirection = direction;
                body.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
                Debug.Log("Moving in direction: " + direction);
            }
            else
            {
                AvoidObstacle();
                Debug.Log("Obstacle detected, avoiding");
            }
        }
        else if (State == CharacterState.Idle)
        {
            Wander();

            wanderTimerCurrent -= Time.deltaTime;
            if (wanderTimerCurrent <= 0)
            {
                StartSearching();
                Debug.Log("Wander timer expired, switching to Searching");
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = ~LayerMask.GetMask(LayerMask.LayerToName(enemyLayer));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, FollowRange, layerMask);

        Debug.DrawRay(transform.position, directionToPlayer, Color.red);

        if (hit.collider != null)
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            return hit.collider.gameObject == player;
        }

        return false;
    }

    private bool CanMoveInDirection(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f);
        return hit.collider == null || !hit.collider.CompareTag("Obstacle");
    }

    private void AvoidObstacle()
    {
        Vector3 rightDirection = Quaternion.Euler(0, 0, 90) * transform.up;
        Vector3 leftDirection = Quaternion.Euler(0, 0, -90) * transform.up;

        if (CanMoveInDirection(rightDirection))
        {
            body.MovePosition(transform.position + rightDirection * movementSpeed * Time.deltaTime);
            Debug.Log("Avoiding obstacle by moving right");
        }
        else if (CanMoveInDirection(leftDirection))
        {
            body.MovePosition(transform.position + leftDirection * movementSpeed * Time.deltaTime);
            Debug.Log("Avoiding obstacle by moving left");
        }
        else
        {
            StartSearching();
            SetState(CharacterState.Idle);
            Debug.Log("Obstacle in all directions, switching to search mode");
        }
    }

    private void SetAnimation()
    {
        animator.SetBool("IsMoving", State == CharacterState.Run);
    }

    private void UpdateSpriteDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void StartSearching()
    {
        isSearching = true;
        searchTimer = searchDuration;
        lastKnownPlayerPosition = player.transform.position;
        Debug.Log("Player out of sight, starting search mode");
    }

    private void StartWandering()
    {
        isSearching = false;
        wanderTimerCurrent = wanderTimer;

        Vector2 randomDirection = Random.insideUnitCircle.normalized * wanderRadius;
        lastKnownPlayerPosition = transform.position + (Vector3)randomDirection;

        SetState(CharacterState.Run);
    }

    private void Wander()
    {
        Vector3 direction = (lastKnownPlayerPosition - transform.position).normalized;
        if (CanMoveInDirection(direction))
        {
            currentDirection = direction;
            body.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        }
        else
        {
            AvoidObstacle();
        }
    }

    private void AttackPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(Damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FollowRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        if (player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.blue : Color.yellow;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }

        if (isSearching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, lastKnownPlayerPosition);
        }

        Vector3 forwardDirection = transform.up;
        Vector3 rightDirection = Quaternion.Euler(0, 0, 90) * transform.up;
        Vector3 leftDirection = Quaternion.Euler(0, 0, -90) * transform.up;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + forwardDirection * 1f);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection * 1f);
        Gizmos.DrawLine(transform.position, transform.position + leftDirection * 1f);
    }
}
