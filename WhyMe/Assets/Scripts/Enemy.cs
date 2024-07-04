using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int Damage = 10;
    public float MinMovementSpeed = 1f;
    public float MaxMovementSpeed = 3f;

    public float AttackRange = 3f; // Attack range
    public float FollowRange = 20f; // Follow range
    public float searchDuration = 5f; // Duration for searching for the player
    public float wanderRadius = 10f; // Radius for wandering area
    public float wanderTimer = 10f; // Time to wander before searching again

    GameObject player;

    private Vector3 lastKnownPlayerPosition;
    private bool isSearching;
    private float searchTimer;
    private float wanderTimerCurrent;

    private Vector3 currentDirection;

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementSpeed = Random.Range(MinMovementSpeed, MaxMovementSpeed);
        animator = GetComponent<Animator>();

        base.Start();

        searchTimer = searchDuration;
        wanderTimerCurrent = wanderTimer;
        StartWandering();
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Distance to player: " + distanceToPlayer);

        if (distanceToPlayer < AttackRange)
        {
            // Attack logic can be added here - for later use
            SetState(CharacterState.Idle);
            Debug.Log("Player within attack range, switching to Idle");
        }
        else if (distanceToPlayer < FollowRange && CanSeePlayer())
        {
            SetState(CharacterState.Run);
            lastKnownPlayerPosition = player.transform.position;
            isSearching = false;
            currentDirection = (player.transform.position - transform.position).normalized;
            UpdateSpriteDirection(currentDirection);
            Debug.Log("Player within follow range and visible, switching to Run");
        }
        else if (isSearching)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0)
            {
                StartWandering();
                Debug.Log("Search timer expired, switching to Wander");
            }
            else
            {
                SetState(CharacterState.Run);
                currentDirection = (lastKnownPlayerPosition - transform.position).normalized;
                UpdateSpriteDirection(currentDirection);
                Debug.Log("Searching for player, moving to last known position");
            }
        }
        else
        {
            Wander();
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
                direction = currentDirection;
            }

            if (CanMoveInDirection(direction))
            {
                body.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
                Debug.Log("Moving in direction: " + direction);
            }
            else
            {
                AvoidObstacle();
                Debug.Log("Obstacle detected, avoiding");
            }
        }

        // Handle wandering logic separately within FixedUpdate to ensure it moves consistently
        if (State == CharacterState.Run && !isSearching)
        {
            wanderTimerCurrent -= Time.deltaTime;
            if (wanderTimerCurrent <= 0)
            {
                StartWandering();
                Debug.Log("Wander timer expired, choosing new direction");
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = ~LayerMask.GetMask(LayerMask.LayerToName(enemyLayer)); // Exclude "Enemy" layer

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
        // Rotate the sprite to face the correct direction
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
        currentDirection = (lastKnownPlayerPosition - transform.position).normalized;
        UpdateSpriteDirection(currentDirection);
        Debug.Log("Started wandering, new direction: " + currentDirection);
    }

    private void Wander()
    {
        if (CanMoveInDirection(currentDirection))
        {
            body.MovePosition(transform.position + currentDirection * movementSpeed * Time.deltaTime);
            UpdateSpriteDirection(currentDirection);
            Debug.Log("Wandering in direction: " + currentDirection);
        }
        else
        {
            AvoidObstacle();
            Debug.Log("Wandering encountered obstacle, avoiding");
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
