using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.Progress;

public class DraggableBox : MonoBehaviour
{
    public bool isBeingDragged = false;
    private Player player; //Reference to player for dragging
    private Rigidbody2D rigidBody;//Component for physics interactions

    public bool isLocked { get; private set; } = false;//Whether the box is locked in place
    public bool IsBeingDragged { get { return isBeingDragged; } }
    private Vector3 lockedPosition; //Store the locked position for stability

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
        if (!isBeingDragged && !isLocked) //Only grab if not locked
        {
            //Debug.Log("BOX GRABBED!");
            isBeingDragged = true;
            this.player = player; //Set the player reference
            rigidBody.bodyType = RigidbodyType2D.Kinematic; //Set back to Kinematic while dragging
        }
    }

    //Method for the Player when they release the box
    public void ReleaseBox()
    {
        if (!isLocked) //Only release if not locked
        {
            //Debug.Log("BOX RELEASED");
            isBeingDragged = false;
            player = null; //Clear the player reference
        }
    }
   
    private void Update()
    {
        //If the box is dragged and the player is moving, follow the player's position
        if (isBeingDragged && player != null)
        {
            if (player.IsMoving) // Check if the player is moving
            {
                FollowPlayer();
            }
        }
        else if (isLocked)
        {
            //Keep the box locked in its position to prevent slight movements
            transform.position = lockedPosition;
        }
    }

    private void FollowPlayer()
    {
        //Calculate the new position of the box relative to the player
        Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;
        Vector3 targetPosition = player.transform.position + directionToPlayer * 1.5f; //Maintain a small distance
        
        //Smoother box position towards the target position
        rigidBody.MovePosition(Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime));
    }

    //Lock the box in place
    public void LockPosition(Vector3 position)
    {
        if (!isLocked)
        {
            isLocked = true;
            isBeingDragged = false; //Stop dragging
            player = null; //Clear player reference
            rigidBody.bodyType = RigidbodyType2D.Static; //Make the box static so it cannot be moved
            lockedPosition = position; //Set the locked position
            transform.position = lockedPosition; //Set the box to the exact position
            //Debug.Log("BOX IS NOW LOCKED IN POSITION");
        }
    }

    //Unlock the box
    public void Unlock()
    {
        if (isLocked)
        {
            isLocked = false;
            rigidBody.bodyType = RigidbodyType2D.Kinematic; //Make the box movable again
            //Debug.Log("BOX IS NOW UNLOCKED");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isLocked)
        {
            //Interaction with enemy -> allow pushing
            Rigidbody2D boxRigidbody = GetComponent<Rigidbody2D>();
            if (boxRigidbody != null)
            {
                Vector2 pushDirection = collision.relativeVelocity.normalized;
                boxRigidbody.AddForce(pushDirection * 500f); // Adjust force based on enemy collision strength
            }
        }
    }
}
