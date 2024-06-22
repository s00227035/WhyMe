using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    float horizontal, vertical;//input horizontal and vertical
    Vector3 mouseWorldPosition;

    public Animator animator;

    public override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();//call the Start method of Character
    }

    private void SetAnimation()
    {
        bool isMoving = horizontal != 0 || vertical != 0;
        animator.SetBool("IsMoving", isMoving);
    }
 



    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        transform.up = mouseWorldPosition - transform.position;

        if (horizontal != 0 || vertical != 0)
        {
            SetState(CharacterState.Run);
        }
        else
        {
            SetState(CharacterState.Idle);
        }

        SetAnimation(); //Set animation based on movement input

    }

    private void FixedUpdate()
    {
        body.MovePosition(transform.position + new Vector3(horizontal, vertical, 0) * movementSpeed * Time.deltaTime);
    }
}
