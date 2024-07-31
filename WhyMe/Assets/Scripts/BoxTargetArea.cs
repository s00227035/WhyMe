using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTargetArea : MonoBehaviour
{
    public bool isOccupied { get; private set; } = false; //Whether the area is occupied by a box
    public string requiredBoxTag = "Box"; //Tag to identify boxes
    private DraggableBox currentBox; //Keep track of the box currently occupying this area
    private TargetAreaColor colorChange; //Reference to the color change script
    //Delay to ensure stability before marking the box as removed
    private const float unoccupiedDelay = 0.2f;

    private void Start()
    {
        //Get the TargetAreaColor component
        colorChange = GetComponent<TargetAreaColor>();
        if (colorChange != null)
        {
            colorChange.SetUnoccupiedColor(); //Initialize color
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOccupied && collision.CompareTag(requiredBoxTag))
        {
            //Mark this area as occupied if the correct box enters
            DraggableBox box = collision.GetComponent<DraggableBox>();
            if (box != null && !box.isLocked) //Ensure the box is not already locked
            {
                isOccupied = true;
                currentBox = box;
                currentBox.LockPosition(transform.position); //Lock the box in position
                                                             //Debug.Log($"Box placed correctly in target area: {gameObject.name}");

                if (colorChange != null)
                {
                    colorChange.SetOccupiedColor(); //Change color
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isOccupied && collision.CompareTag(requiredBoxTag))
        {
            // Ensure that the box is the same one leaving and hasn't been dragged out intentionally
            DraggableBox box = collision.GetComponent<DraggableBox>();
            if (box != null && box == currentBox)
            {
                // Start a coroutine to delay the unoccupying process
                StartCoroutine(CheckAndUnlock(box));
            }
        }
    }

    // Coroutine to handle the delay before marking as unoccupied
    private IEnumerator CheckAndUnlock(DraggableBox box)
    {
        yield return new WaitForSeconds(unoccupiedDelay);

        // Ensure that the box is no longer within the target area
        if (currentBox == box && !box.isBeingDragged && !IsBoxWithinTargetArea(box))
        {
            box.Unlock(); // Unlock the box when it leaves the area
            currentBox = null;
            isOccupied = false;
            //Debug.Log($"Box removed from target area: {gameObject.name}");

            if (colorChange != null)
            {
                colorChange.SetUnoccupiedColor(); //Revert color to unoccupied
            }
        }
    }

    // Check if the box is still within the target area
    private bool IsBoxWithinTargetArea(DraggableBox box)
    {
        Bounds targetBounds = GetComponent<Collider2D>().bounds;
        return targetBounds.Contains(box.transform.position);
    }
}
