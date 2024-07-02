using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int Damage = 10;
    public float MinMovementSpeed = 1f;
    public float MaxMovementSpeed = 3f;

    public float AttackRange = 3f;//Attack range
    public float FollowRange = 20f;//Follow range
    public float searchDuration = 5f;//duration for searching of the player
    GameObject player;


    private Vector3 lastKnownPlayerPosition;
    private bool isSearching;
    private float searchTimer;

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementSpeed = Random.Range(MinMovementSpeed, MaxMovementSpeed);
        animator = GetComponent<Animator>();

        base.Start();
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Distance to player: " + distanceToPlayer);

        if (distanceToPlayer < AttackRange)
        {
            //Attack logic can be added here - for later use
            SetState(CharacterState.Idle);
            Debug.Log("Player within attack range, switching to Idle");
        }
        //Player is seen and then searching is not true
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
                Debug.Log("Player within follow range and visible, switching to Run");
            }
            else
            {
                StartSearching();
                Debug.Log("Player within follow range but not visible, starting search mode");
            }
        }
        //Player is not seen and then searching is true
        else if (isSearching)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0)
            {
                SetState(CharacterState.Idle);
                isSearching = false;
                Debug.Log("Search timer expired, switching to Idle");
            }
            else
            {
                SetState(CharacterState.Run);
                UpdateSpriteDirection(lastKnownPlayerPosition - transform.position);
                Debug.Log("Searching for player, moving to last known position");
            }
        }
        //Else animation Idle
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
                body.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
                Debug.Log("Moving in direction: " + direction);
            }
            else
            {
                //Handle obstacle avoidance here
                AvoidObstacle();
                Debug.Log("Obstacle detected, avoiding");
            }
        }
    }

    


    //BOOL - CAN SEE PLAYER
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

    //BOOL - CAN MOVE IN DIRECTION
    private bool CanMoveInDirection(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f);
        Debug.DrawRay(transform.position, direction, Color.green);
        return hit.collider == null || !hit.collider.CompareTag("Obstacle");
    }

    //DETECT OBSTACLE AND AVOID IT
    private void AvoidObstacle()
    {
        //Basic obstacle avoidance by trying to move in a specific direction
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
            //No valid direction found, switch to Idle and start searching
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
        Debug.Log("Player out of sight, starting search mode");
    }

    //GIZMOS FOR DEBUGGING
    private void OnDrawGizmos()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        // Draw follow range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FollowRange);

        // Draw direction ray
        if (player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.blue : Color.yellow;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }

        // Draw search direction ray
        if (isSearching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, lastKnownPlayerPosition);
        }

        // Draw obstacle avoidance rays
        Vector3 forwardDirection = transform.up;
        Vector3 rightDirection = Quaternion.Euler(0, 0, 90) * transform.up;
        Vector3 leftDirection = Quaternion.Euler(0, 0, -90) * transform.up;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + forwardDirection * 1f);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection * 1f);
        Gizmos.DrawLine(transform.position, transform.position + leftDirection * 1f);
    }
}
