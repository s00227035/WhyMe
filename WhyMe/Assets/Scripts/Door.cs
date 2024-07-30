using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public BoxTargetArea[] targetAreas; //Array of target areas

    private bool isOpen = false; //Keep track of whether the door is already open

    private void Update()
    {
        if (!isOpen && AreAllBoxesPlaced())
        {
            OpenDoor();
        }
    }

    //Check if all boxes are placed in their respective areas
    private bool AreAllBoxesPlaced()
    {
        foreach (BoxTargetArea area in targetAreas)
        {
            if (!area.isOccupied)
            {
                //Debug.Log($"TARGET AREA {area.gameObject.name} IS NOT OCCUPIED");
                return false; //Return not true if any area is not occupied
            }
        }
        return true; //Return true if areas are occuped
    }

    //Open the door
    private void OpenDoor()
    {
        Debug.Log("DOOR OPENED!");
        isOpen = true;
        Destroy(gameObject); //Destroy the door
    }
}
