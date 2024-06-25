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

    //Setting up animation based on bool
    private void SetAnimation()
    {
        bool isMoving = horizontal != 0 || vertical != 0;
        animator.SetBool("IsMoving", isMoving);
    }
 


    //Movement + states
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
            //Update sprite direction
            UpdateSpriteDirection(horizontal, vertical);
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

    private void UpdateSpriteDirection(float horizontal, float vertical)
    {
        //Determine direction based on input
        Vector3 movementDirection = new Vector3(horizontal, vertical, 0).normalized;

        //Assuming the default direction of the sprite is facing north (up)
        if (movementDirection != Vector3.zero)
        {
            //Determine the angle in degrees to rotate the sprite, adjusting for the north-facing default
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg -90f;

            //Apply rotation to the sprite
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
