using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBox : MonoBehaviour
{
    public bool isBeingDragged = false;
    private Player player; //Reference to player for dragging
    private Rigidbody2D rigidBody;//Component for physics interactions

    private bool isLocked = false;//Whether the box is locked in place

    private void Start()
    {
        //Get the Rigidbody2D component attached to the box
        rigidBody = GetComponent<Rigidbody2D>();
        //Ensure the Rigidbody is kinematic for manual control
        if (rigidBody != null)
        {
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; //Prevent rotation for top-down view
        }
    }

    //Method for the Player when they grab the box
    public void GrabBox(Player player)
    {
        if (!isBeingDragged)
        {
            //Debug.Log("BOX GRABBED!");
            isBeingDragged = true;
            this.player = player; //Set the player reference
        }
    }

    //Method for the Player when they release the box
    public void ReleaseBox()
    {
        //Debug.Log("BOX RELEASED");
        isBeingDragged= false;
        player = null; //Clear the player reference
    }
   
    private void Update()
    {
        //If the box is dragged and the player is moving, follow the player's position
        if (isBeingDragged && player != null)
        {
            if (player.IsMoving) //Check if the player is moving
            {
                FollowPlayer();
            }
        }
    }

    //Lock the box in place
    private void FollowPlayer()
    {
        //Calculate the new position of the box relative to the player
        Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;
        Vector3 targetPosition = player.transform.position + directionToPlayer * 1.5f; //Maintain a small distance
        
        //Smoother box position towards the target position
        rigidBody.MovePosition(Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime));
    }

    public void LockPosition(Vector3 position)
    {
        isLocked = true;
        rigidBody.MovePosition(position);
    }
}
