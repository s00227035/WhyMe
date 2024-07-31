using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    private float horizontal, vertical;//input horizontal and vertical
    Vector3 mouseWorldPosition;
    private DraggableBox draggableBox = null;//Reference to the draggable box
    private const float interactionRange = 2f;//interaction range for grabbing boxes
    public bool IsMoving {  get; private set; }
    private bool inputEnabled = true; //Enable/disable player input

    public override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();//call the Start method of Character
    }

    //method to enable/disable input
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    //Setting up animation based on bool
    private void SetAnimation()
    {
        bool isMoving = horizontal != 0 || vertical != 0;
        animator.SetBool("IsMoving", isMoving);
        IsMoving = isMoving;//Set the movement state
    }
 


    //Movement + states
    private void Update()
    {
        if (!inputEnabled)
        {
            return; //Exit early if input is disabled
        }

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
        HandleBoxInteraction();//Check for box interactions

    }

    private void FixedUpdate()
    {
        if (!inputEnabled)
        {
            return; //Exit early if input is disabled
        }

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

    //Handle interaction with boxes
    private void HandleBoxInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (draggableBox == null)
            {
                TryGrabBox();
            }
            else
            {
                //Release the current box
                //Debug.Log("RELEASING BOX.");
                draggableBox.ReleaseBox();
                draggableBox = null;
            }
        }
    }

    //Attempt to grab a box
    private void TryGrabBox()
    {
        //Check for boxes within range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        //Debug.Log($"Checking for boxes within range {interactionRange}..."); // Log range check

        DraggableBox closetBox = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            DraggableBox box = collider.GetComponent<DraggableBox>();
            if (box != null && !box.isBeingDragged)
            {
                //Calculate the distance to the player
                float distance = Vector2.Distance(transform.position, box.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closetBox = box;
                }
            }
        }

        if (closetBox != null)
        {
            //Debug.Log("BOX FOUND! ATTEMPTING TO GRAB..");
            draggableBox = closetBox;
            draggableBox.GrabBox(this);
        }
    }

    //Visualize the range for interaction
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);//Range
    }

}
