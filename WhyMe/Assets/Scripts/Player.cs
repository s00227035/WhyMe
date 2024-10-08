using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    private float horizontal, vertical;//input horizontal and vertical
    private Vector3 mouseWorldPosition;
    private DraggableBox draggableBox = null;//Reference to the draggable box
    private const float interactionRange = 2f;//interaction range for grabbing boxes
    public bool IsMoving { get; private set; }
    private bool inputEnabled = true; //Enable/disable player input
    private Collider2D playerCollider; //Reference to the player collider
    //Audio
    private AudioSource audioSource;
    public AudioClip walkingSound;

    public override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();//call the Start method of Character

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>(); //Initialize player collider
        }

        //Initialize audio source
        audioSource = GetComponent<AudioSource>();

        //Loop the walking sound
        if (audioSource != null && walkingSound != null)
        {
            audioSource.clip = walkingSound;
            audioSource.loop = true; //Looping
        }
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

        //Walking sound
        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play(); //Sound if moving
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop(); //No sound if Idle
        }
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

        //OLD
        transform.up = mouseWorldPosition - transform.position;

        //NEW differnt version for 360 angle
        //Rotate player to face the mouse position
        //RotateTowardsMouse();

        if (horizontal != 0 || vertical != 0)
        {
            SetState(CharacterState.Run);
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

        //Move the player
        Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;
        body.MovePosition(transform.position + moveDirection * movementSpeed * Time.deltaTime);
    }

    /* 360 ANGLE ROTATION
    //NEW
    //Rotate the player towards the mouse position
    private void RotateTowardsMouse()
    {
        // Calculate the direction to look towards the mouse
        Vector3 lookDirection = mouseWorldPosition - transform.position;

        // Get the angle in radians and convert it to degrees
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;

        // Create the target rotation and apply it
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = targetRotation;
    }
    */

    //OLD
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
                ReleaseBox();
                
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

            //Disable collision between player and box
            Physics2D.IgnoreCollision(playerCollider, draggableBox.GetComponent<Collider2D>(), true);
        }
    }

    //ReleaseBox again
    private void ReleaseBox()
    {
        if (draggableBox != null)
        {
            //Re-enable collision between player and box
            Physics2D.IgnoreCollision(playerCollider,draggableBox.GetComponent<Collider2D>(), false);
            draggableBox.ReleaseBox();
            draggableBox=null;
        }
    }


    //Visualize the range for interaction
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);//Range
    }

}
