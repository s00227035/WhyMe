using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int Damage = 10;
    public float MinMovementSpeed = 1;
    public float MaxMovementSpeed = 3;

    public float AttackRange = 3;//Attack range
    public float FollowRange = 10;//Follow range
    GameObject player;

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

        if (distanceToPlayer < AttackRange)
        {
            //Attack logic can be added here - for later use
            SetState(CharacterState.Idle);
        }
        else if (distanceToPlayer < FollowRange)
        {
            SetState(CharacterState.Run);
            transform.up = player.transform.position - transform.position;
        }
        else
        {
            SetState(CharacterState.Idle);
        }

        SetAnimation();
    }

    private void FixedUpdate()
    {
        if (State == CharacterState.Run)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            body.MovePosition(transform.position + transform.up.normalized * movementSpeed * Time.deltaTime);
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
}
