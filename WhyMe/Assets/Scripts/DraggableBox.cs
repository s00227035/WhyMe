using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBox : MonoBehaviour
{
    private bool isBeingDragged = false;
    private Player player; //Reference to player for dragging
    
    //Method for the Player when they grab the box
    public void GrabBox(Player player)
    {
        if (!isBeingDragged)
        {
            isBeingDragged = true;
            this.player = player; //Set the player reference
        }
    }

    //Method for the Player when they release the box
    public void ReleaseBox()
    {
        isBeingDragged= false;
        player = null; //Clear the player reference
    }
   
    private void Update()
    {
        //If the box is dragged, follow the player's position
        if (isBeingDragged && player != null)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        //Calculate the new position of the box relative to the player
        Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;
        Vector3 targetPosition = player.transform.position + directionToPlayer * 1.5f; //Maintain a small distance
        
        //Smoother box position towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
    }
}
